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
        moveGoblins();
        updateWizards();
        recoverWizards();
        checkForBackup();

    }
    protected void updateTimer()
    {
        nextSpellTime = (nextSpellTime == 0) ? randomSpellTime() : nextSpellTime - 1;
    }
    protected void updateSpells()
    {
        Node<Spell>? currentSpell = Spells.Head!;
        while (currentSpell != null)
        {
            currentSpell.Item.Move(0, -Spell.Speed);
            if (CastleGameRenderer.IsOffScreen(currentSpell.Item.Position)) //if the spell is off the screen, remove it
            {
                Spells!.Remove(currentSpell.Item);
            };
            currentSpell = currentSpell.Next;
        }

    }
    protected void moveHeadGoblin(Node<Goblin> headGoblin)
    {
        headGoblin.Item.Move(goblinDirection, Goblin.Speed);
        Vector2 localGoblinDirection = goblinDirection;
        CastleGameRenderer.CheckWallCollision(headGoblin.Item, ref localGoblinDirection);
        goblinDirection = localGoblinDirection;
    }
    protected void moveGoblins()
    {
        Node<Goblin>? headGoblin = GoblinSquad.Head;
        if (headGoblin == null)
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
                // Console.WriteLine($"Goblin hit at position {goblinNode.Item.Position}, removing goblin and spell.");
                GoblinSquad.Remove(goblinNode.Item);
                Spells.Remove(currentSpell.Item);
                goblinDirection = randomGoblinDirection();
                return; // Exit the method as the goblin is removed
            }
            currentSpell = currentSpell.Next;
        }
    }

    protected void updateWizards()
    {
        if (nextSpellTime == 0)
        {
            if (ActiveWizard != null && !WizardSquad.IsEmpty)
            {
                Spell wizardSpell = new Spell(ActiveWizard.Item.SpellType, ActiveWizard.Item.Position);
                Spells.AddBack(wizardSpell);
                if (ActiveWizard.Item.Energy < ActiveWizard.Item.SpellLevel)
                {
                    ActiveWizard.Item.Energy = 0;
                }
                else
                {
                    ActiveWizard.Item.Energy -= ActiveWizard.Item.SpellLevel;
                }

                if (ActiveWizard.Item.Energy == 0)
                {
                    WizardSquad.Remove(ActiveWizard);
                    RecoveryQueue.Enqueue(ActiveWizard.Item);
                }


                if (ActiveWizard.Next != null)
                {
                    ActiveWizard = ActiveWizard.Next;
                }
                else
                {
                    ActiveWizard = WizardSquad.Head;
                }
                nextSpellTime = randomSpellTime();

            }
        }
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