# Wizards and Goblins Game - 2
1. Iterate through the list of Spells and update each one.
1. Move them up the screen. Note that "up" on the screen is the negative Y-direction. Spells have a static property for speed that you can use to determine how fast they moves.
2. If the spell flies off the top of the screen, it should be removed from the list of spells. You can call the CastleGameRenderer.IsOffScreen() method to check if the spell has flown off the top yet.
2. Iterate & update the list of Goblins.
1. Move the goblins.
• The first goblin (the one at Head) should move according to goblinDirection.
• All of the other goblins follow the one in front of them.
2. Loop over all the spells and check for any collisions. If any spell collides with a goblin, both of them should be removed from their respective lists. When any goblin in the squad gets hit, the lead goblin should pick a new goblinDirection.
3. Update the Wizards.
1. If the nextSpellTime timestamp has passed, it's time for the currently active wizard to cast a spell.
• Casting a spell involves creating a new Spell object at the wizard's current position with the wizard's SpellType, and then decrementing the wizard's Energy by their SpellLevel (kind of like the "cost" of the spell).
• Once a wizard casts their spell, the next wizard in the chain becomes the "active" wizard, and they wait for their turn (be sure to actually re-set your nextSpellTimer).
2. If a wizard runs out of energy, they should fall-out of the front lines and get into the recovery queue to replenish their Energy.
3. The first wizard in the recovery queue (if applicable), should take a bite out of the food in front of them every 5 or so frames, replenishing their Energy by one point.
4. Once the first wizard in the recovery queue is finished eating (their energy hits its max), they should excuse themselves from the queue and rejoin the frontlines immediately before the currently active wizard.
4. Check if the Goblins need reinforcements yet.
1. Remember, the six extra goblins from the backup list show up when around half of the original group is remaining. Use your AppendAll() method to simply merge the backup goblins directly onto the main group.
5. Check if the Goblins have been defeated yet.
1. If all the goblins are gone, you can call the Pause() method (inherited from the Visualization base class) to stop the simulation. Or you can keep it running, if you want. But you should write a message to the console at the very least.
### Gameplay video: 
https://github.com/user-attachments/assets/e09f45f1-ae80-45bb-a7de-fe04abf0546b

