//------------------------------------------------------------------------------------------
/*  TEXT OBJECT
*/
//------------------------------------------------------------------------------------------
using System.Numerics;
using System.Text;
using Raylib_cs;

namespace Topdown.GUI {
    /// <summary>
    /// Text Object
    /// </summary>
    public class TextObject : UIEntity {
		// PROPERTIES
		//------------------------------------------------------------------------------------------
		public String Text { get; set; }
		public TextStyles TextStyle { get; private set; }

		/// <summary>
		/// Regular Constructor
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="size"></param>
		/// <param name="text"></param>
		/// <param name="textStyle"></param>
		/// <param name="bgColor"></param>
		public TextObject(Vector2 pos, Vector2 size, String text, TextStyles textStyle, Color? bgColor) : base(pos, size, bgColor) {
			Text = text;
			TextStyle = textStyle;
		}

		// FUNCTIONS
		//------------------------------------------------------------------------------------------
		public override void Render() {
			base.Render();
			
			// int drawY = (int)_absoluteRect.y;

			// TODO: Text Vertical alignment, to work with multiple lines of string
			// switch (TextStyle.VerticalAlign) {	
			// 	case VerticalAlignment.Top:
			// 		drawY += (int)TextStyle.Margin.Y;
			// 		break;
			// 	case VerticalAlignment.Center:
			// 		if (TextStyle.Size <= _absoluteRect.height)
			// 			drawY = (int)(_absoluteRect.y + ((_absoluteRect.height - TextStyle.Size) / 2));
			// 			// drawY += (int)Font.Margin.Y;
			// 		break;
			// 	case VerticalAlignment.Bottom:
			// 		// TODO: This
			// 		throw new Exception("No Vertical Align Bottom Setting");
			// 	default:
			// 		break;
			// }

			Rectangle drawRect = new Rectangle(_absoluteRect.x + TextStyle.Margin.X,
												_absoluteRect.y + TextStyle.Margin.Y,
												Rect.width - TextStyle.Margin.Z,
												Rect.height - TextStyle.Margin.W);

			DrawTextWrap(Text, drawRect, TextStyle.Size, TextStyle);	
		}


		
		private unsafe static void DrawTextWrap(string text, Rectangle rect, int size, TextStyles TextStyle) {
			// THIS FUNCTION MIGHT BE THE ONLY REASON WHY I'M COMPILING IN UNSAFE
			// This entire function COPIED
			// Reference: https://www.raylib.com/examples.html
			//			https://github.com/ChrisDill/Raylib-cs/blob/master/Examples/Text/RectangleBounds.cs

			// Modifications: I set WordWrap to always TRUE (some simplificaitions because of that)
			//				Also removed selectStart? and rect select bg color? stuff like that? yeah idk

			int length = text.Length;

			// Offset between lines (on line break '\n')
			float textOffsetY = 0;

			// Offset X to next character to draw
			float textOffsetX = 0.0f;

			// Character rectangle scaling factor
			float scaleFactor = size / (float)TextStyle.Font.baseSize;

			// Word/character wrapping mechanism variables
			bool shouldMeasure = true;

			// Index where to begin drawing (where a line begins)
			int startLine = -1;

			// Index where to stop drawing (where a line ends)
			int endLine = -1;

			// Holds last value of the character position
			int lastk = -1;

			using var textNative = new UTF8Buffer(text);

			for (int i = 0, k = 0; i < length; i++, k++) {
				// Get next codepoint from byte string and glyph index in font
				int codepointByteCount = 0;
				int codepoint = Raylib.GetCodepoint(&textNative.AsPointer()[i], &codepointByteCount);
				int index = Raylib.GetGlyphIndex(TextStyle.Font, codepoint);

				// NOTE: Normally we exit the decoding sequence as soon as a bad byte is found (and return 0x3f)
				// but we need to draw all of the bad bytes using the '?' symbol moving one byte
				if (codepoint == 0x3f) {
					codepointByteCount = 1;
				}

				i += codepointByteCount - 1;

				float glyphWidth = 0;
				if (codepoint != '\n') {
					glyphWidth = (TextStyle.Font.glyphs[index].advanceX == 0) ?
					TextStyle.Font.recs[index].width * scaleFactor :
					TextStyle.Font.glyphs[index].advanceX * scaleFactor;

					if (i + 1 < length) {
						glyphWidth += 2.0f; // 2.0 = spacing
					}
				}

				// NOTE: When wordWrap is ON we first measure how much of the text we can draw before going outside of
				// the rec container. We store this info in startLine and endLine, then we change states, draw the text
				// between those two variables and change states again and again recursively until the end of the text
				// (or until we get outside of the container). When wordWrap is OFF we don't need the measure state so
				// we go to the drawing state immediately and begin drawing on the next line before we can get outside
				// the container.
				if (shouldMeasure) {
					// TODO: There are multiple types of spaces in UNICODE, maybe it's a good idea to add support for
					// more. Ref: http://jkorpela.fi/chars/spaces.html
					if ((codepoint == ' ') || (codepoint == '\t') || (codepoint == '\n')) {
						endLine = i;
					}

					if ((textOffsetX + glyphWidth) > rect.width) {
						endLine = (endLine < 1) ? i : endLine;
						if (i == endLine) {
							endLine -= codepointByteCount;
						}

						if ((startLine + codepointByteCount) == endLine) {
							endLine = (i - codepointByteCount);
						}

						shouldMeasure = !shouldMeasure;
					} else if ((i + 1) == length) {
						endLine = i;
						shouldMeasure = !shouldMeasure;
					} else if (codepoint == '\n') {
						shouldMeasure = !shouldMeasure;
					}

					if (!shouldMeasure) {
						textOffsetX = 0;
						i = startLine;
						glyphWidth = 0;

						// Save character position when we switch states
						int tmp = lastk;
						lastk = k - 1;
						k = tmp;
					}
				} else {
					if (codepoint == '\n') {
						// if (false){
						// 	textOffsetY += (TextStyle.Font.baseSize + TextStyle.Font.baseSize / 2) * scaleFactor;
						// 	textOffsetX = 0;
						// }
					} else {
						if ((textOffsetX + glyphWidth) > rect.width) {
							textOffsetY += (TextStyle.Font.baseSize + TextStyle.Font.baseSize / 2) * scaleFactor;
							textOffsetX = 0;
						}

						// When text overflows rectangle height limit, just stop drawing
						if ((textOffsetY + TextStyle.Font.baseSize * scaleFactor) > rect.height) {
							break;
						}

						// Draw selection background
						// bool isGlyphSelected = false;
						// if ((selectStart >= 0) && (k >= selectStart) && (k < (selectStart + selectLength))) {
						// 	// Raylib.DrawRectangleRec(new Rectangle(rec.X + textOffsetX - 1, rec.Y + textOffsetY, glyphWidth, (float)TextStyle.Font.baseSize * scaleFactor), selectBackTint);
						// 	isGlyphSelected = true;
						// }

						// Draw current character glyph
						if ((codepoint != ' ') && (codepoint != '\t')) {
							Raylib.DrawTextCodepoint(TextStyle.Font, codepoint, new Vector2(rect.x + textOffsetX, rect.y + textOffsetY), size, TextStyle.Color);
						}
					}

					if (i == endLine) {
						textOffsetY += (TextStyle.Font.baseSize + TextStyle.Font.baseSize / 2) * scaleFactor;
						textOffsetX = 0;
						startLine = endLine;
						endLine = -1;
						glyphWidth = 0;
						// selectStart += lastk - k;
						k = lastk;

						shouldMeasure = !shouldMeasure;
					}
				}

				if ((textOffsetX != 0) || (codepoint != ' ')) {
					// avoid leading spaces
					textOffsetX += glyphWidth;
				}
			}
		}
	}
}
