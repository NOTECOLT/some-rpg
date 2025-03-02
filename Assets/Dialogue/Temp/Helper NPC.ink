-> main

EXTERNAL equipWeapon(weaponid, playerid)
=== function equipWeapon(weaponid, playerid) ===
~ return

=== main ===
VAR choiceid = ""
VAR choiceName = ""
VAR choicePlayer = 0
VAR playerName = ""
Whose weapon would you like to change?
    * [Jim]
        ~ choicePlayer = 0
        ~ playerName = "Jim"
    * [Pam]
        ~ choicePlayer = 1
        ~ playerName = "Pam"
    * [Roy]
        ~ choicePlayer = 2
        ~ playerName = "Roy"
        
- What would you like to change your weapon to?
    * [Basic Blade]
        ~ choiceid = "basicblade"
        ~ choiceName = "Basic Blade"
    * [Mallet]
        ~ choiceid = "mallet"
        ~ choiceName = "Mallet"
    * [Simple Bow]
        ~ choiceid = "simplebow"
        ~ choiceName = "Simple Bow"
    * [Wooden Staff]
        ~ choiceid = "woodenstaff"
        ~ choiceName = "Wooden Staff"
    * [Exit]
        -> END

- Changed {playerName}'s weapon to {choiceName}!
~ equipWeapon(choiceid, choicePlayer)

-> END