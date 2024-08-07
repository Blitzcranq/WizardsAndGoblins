namespace COIS2020.vrajchauhan.Assignment3;

using Microsoft.Xna.Framework; // Needed for Vector2
using TrentCOIS.Tools.Visualization;
using COIS2020.StarterCode.Assignment3;
using System.Linq;

public class CastleDefender : Visualization
{
    public LinkedList<Wizard> WizardSquad { get; private set; } = new LinkedList<Wizard>();
    public Queue<Wizard> RecoveryQueue { get; private set; } = new Queue<Wizard>();

    public LinkedList<Goblin> GoblinSquad { get; private set; } = new LinkedList<Goblin>();
    public LinkedList<Goblin> BackupGoblins { get; private set; } = new LinkedList<Goblin>();

    public LinkedList<Spell> Spells { get; private set; } = new LinkedList<Spell>();
    public Node<Wizard>? ActiveWizard { get; private set; }
    public Vector2 goblinDirection { get; private set; } = new Vector2(0, 0);
    private uint nextSpellTime = 0;

    protected Vector2 randomGoblinDirection()
    {
        Vector2 direction = new Vector2(RNG.NextSingle() * 2 - 1, RNG.NextSingle() * 2 - 1);
        direction.Normalize();
        return direction;
    }
    protected uint randomSpellTime()
    {
        return (uint)(15 + RNG.Next(-5, 5));
    }
    public CastleDefender()
    {
        for (int i = 0; i < 8; i++)
        {
            WizardSquad.AddBack(new Wizard());
        }
        for (int i = 0; i < 8; i++)
        {
            GoblinSquad.AddBack(new Goblin());
        }
        for (int i = 0; i < 6; i++)
        {
            BackupGoblins.AddBack(new Goblin());
        }
        goblinDirection = randomGoblinDirection();
        ActiveWizard = WizardSquad!.Head;
        nextSpellTime = randomSpellTime();

    }

    protected override void Update(uint currentFrame)
    {
        updateTimer();
        updateSpells();
        updateGoblins();
        updateWizards();

        checkForBackup();

    }
    protected void updateTimer()
    {
        nextSpellTime = (nextSpellTime == 0) ? randomSpellTime() : nextSpellTime - 1;
    }
    // Update the spells
    protected void updateSpells()
    {
        Node<Spell>? currentSpell = Spells.Head!; // Get the head of the list
        while (currentSpell != null) // Loop through all the spells
        {
            currentSpell.Item.Move(0, -Spell.Speed); // Move the spell up (negative y direction)
            if (CastleGameRenderer.IsOffScreen(currentSpell.Item.Position)) //if the spell is off the screen, remove it
            {
                Spells!.Remove(currentSpell.Item);
            };
            currentSpell = currentSpell.Next; // Move to the next spell
        }

    }
    protected void moveHeadGoblin(Node<Goblin> headGoblin)
    {
        headGoblin.Item.Move(goblinDirection, Goblin.Speed);
        Vector2 localGoblinDirection = goblinDirection;
        CastleGameRenderer.CheckWallCollision(headGoblin.Item, ref localGoblinDirection);
        goblinDirection = localGoblinDirection;
    }

    // Update the goblins
    protected void updateGoblins()
    {
        Node<Goblin>? headGoblin = GoblinSquad.Head; // Get the head of the list
        //Checking for game over condition
        if (headGoblin == null) // If the head is null, the game is over and the wizard wins
        {
            Pause();
            Console.WriteLine("Wizard wins!");
            return;
        }
        // Move all other goblins and check for spell collisions
        Node<Goblin>? currentGoblin = GoblinSquad.Tail;
        while (currentGoblin != null && currentGoblin != headGoblin)
        {
            if (currentGoblin.Prev != null)
            {
                currentGoblin.Item.MoveTowards(currentGoblin.Prev.Item, Goblin.Speed);
                CheckSpellCollisions(currentGoblin);
            }
            currentGoblin = currentGoblin.Prev;
        }

        // move head goblin now
        if (headGoblin != null)
        {
            moveHeadGoblin(headGoblin);
            CheckSpellCollisions(headGoblin);
        }
    }

    protected void CheckSpellCollisions(Node<Goblin> goblinNode)
    {
        Node<Spell>? currentSpell = Spells.Head;
        while (currentSpell != null)
        {
            if (currentSpell.Item.Colliding(goblinNode.Item))
            {

                GoblinSquad.Remove(goblinNode.Item);
                Spells.Remove(currentSpell.Item);
                goblinDirection = randomGoblinDirection();
                return; // Exit the method as the goblin is removed
            }
            currentSpell = currentSpell.Next;
        }
    }

    // Update the wizards
    protected void updateWizards()
    {
        if (nextSpellTime == 0) //if the wizard can cast a spell
        {
            if (ActiveWizard != null && !WizardSquad.IsEmpty) //check first if the wizard is not null and the squad is not empty
            {
                Spell wizardSpell = new Spell(ActiveWizard.Item.SpellType, ActiveWizard.Item.Position); //create a new spell
                Spells.AddBack(wizardSpell); //add the spell to the list 
                if (ActiveWizard.Item.Energy < ActiveWizard.Item.SpellLevel) //if the wizard does not have enough energy to cast the spell
                {
                    ActiveWizard.Item.Energy = 0; //set the energy to 0
                }
                else
                {
                    ActiveWizard.Item.Energy -= ActiveWizard.Item.SpellLevel; //otherwise, subtract the energy from the wizard
                }

                if (ActiveWizard.Item.Energy == 0) //if the wizard has no energy left
                {
                    WizardSquad.Remove(ActiveWizard); //remove the wizard from the squad
                    RecoveryQueue.Enqueue(ActiveWizard.Item);   //add the wizard to the recovery queue
                }


                if (ActiveWizard.Next != null) //if the next wizard is not null
                {
                    ActiveWizard = ActiveWizard.Next; //set the active wizard to the next wizard
                }
                else
                {
                    ActiveWizard = WizardSquad.Head; //otherwise, set the active wizard to the head of the squad
                }
                nextSpellTime = randomSpellTime(); //reset the next spell time

            }
        }
        recoverWizards();
    }
    protected void recoverWizards()
    {
        if (!RecoveryQueue.IsEmpty)
        {
            if (nextSpellTime % 5 == 0)
            {
                Wizard currentWizard = RecoveryQueue.Peek();
                currentWizard.Energy += 1;
                checkReJoiningSquad(currentWizard);
            }
        }
    }


    protected void checkReJoiningSquad(Wizard wizard)
    {

        if (wizard.Energy >= wizard.MaxEnergy)
        {
            Wizard wizardToAdd = RecoveryQueue.Dequeue();
            if (WizardSquad.IsEmpty)
            {
                WizardSquad.AddBack(wizardToAdd);
                ActiveWizard = WizardSquad.Head;
            }
            else
            {
                if (ActiveWizard != null)
                    WizardSquad.InsertBefore(ActiveWizard!, wizardToAdd);
                else
                {
                    WizardSquad.AddBack(wizardToAdd);
                    ActiveWizard = WizardSquad.Head;
                }
            }

        }
    }

    protected void checkForBackup()
    {
        if (GoblinSquad.Count <= 4)
        {
            GoblinSquad.AppendAll(BackupGoblins);
        }
    }
}