

typedef struct Player {
	Vector2 position;
	Vector2 speed;
} Player;

/** Updates Player Movement by adding speed to position vectors
**/
void UpdatePlayerVectors(Player* p) {
	p->position.x += p->speed.x;
	p->position.y += p->speed.y;
}