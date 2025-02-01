-> main

EXTERNAL equipWeapon(weaponid)
=== function equipWeapon(weaponid) ===
~ return

=== main ===
VAR choiceid = ""
VAR choiceName = ""
What would you like to change your weapon to?
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

- You got the {choiceName}!
~ equipWeapon(choiceid)

-> END