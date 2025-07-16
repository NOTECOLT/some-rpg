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
    * [Rusty Sword]
        ~ choiceid = "rustysword"
        ~ choiceName = "Rusty Sword"
    * [Club]
        ~ choiceid = "club"
        ~ choiceName = "Club"
    * [Old Bow]
        ~ choiceid = "oldbow"
        ~ choiceName = "Old Bow"
    * [Wooden Staff]
        ~ choiceid = "woodenstaff"
        ~ choiceName = "Wooden Staff"
    * [Exit]
        -> END

- Changed {playerName}'s weapon to {choiceName}!
~ equipWeapon(choiceid, choicePlayer)

-> END