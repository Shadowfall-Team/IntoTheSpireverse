using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Minions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ui;

// Modelling this off of NOrbManager
public partial class NMinionManager : Control
{
    private Control _minionContainer;
    private readonly List<AbstractMinion> _minions = [];
    private NCreature _parentCreature;

    private Player Player => _parentCreature.Entity.Player!;

    public override void _Ready()
    {
        _minionContainer = GetNode<Control>("%Minions");
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
        CombatManager.Instance.CombatSetUp += OnCombatSetup;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
        CombatManager.Instance.CombatSetUp += OnCombatSetup;
    }

    private void OnCombatSetup(CombatState state)
    {
        if (Player.Creature.IsAlive && Player.PlayerCombatState != null)
        {
            // Do something to add the slots and visuals
        }
    }

    private void OnCombatStateChanged(CombatState state)
    {
        foreach (AbstractMinion minion in _minions)
        {
            // update minion visuals if necessary
        }
    }
}

public static class PlayerMinionSlots
{
}

public class MinionSlots(Player owner)
{
    const int DefaultMaxMinions = 3;

    private readonly Player _owner = owner;
    private readonly List<AbstractMinion> _minions = [];
    public IReadOnlyList<AbstractMinion> Minions => _minions;
    public int Capacity { get; private set; }

    public void Clear()
    {
        _minions.Clear();
        Capacity = 0;
    }

    public void AddCapacity(int capacity)
    {
        Capacity += capacity;
    }

    public void RemoveCapacity(int capacity)
    {
        Capacity = Math.Max(0, Capacity - capacity);
        while (Minions.Count > Capacity)
        {
            Remove(_minions.Last());
        }
    }

    public async Task<bool> TryConjure(AbstractMinion minion)
    {
        if (Capacity == 0)
        {
            return false;
        }
        minion.AssertMutable();
        if (Minions.Count >= Capacity)
        {
            throw new InvalidOperationException("Spireverse MinionQueue is full");
        }
        
        return true;
    }

    public async Task<bool> TryConjureInSlot(AbstractMinion minion, int index)
    {

        return true;
    }

    public bool Remove(AbstractMinion minion)
    {
        return _minions.Remove(minion);
    }

    public void Insert(int index, AbstractMinion minion)
    {
        if (index >= Capacity)
        {
            throw new InvalidOperationException("index cannot be greater than minion capacity");
        }
        _minions.Insert(index, minion);
    }

    private async Task SmallWait()
    {
        if (LocalContext.IsMe(_owner))
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
        else
        {
            await Cmd.Wait(0.05f);
        }
    }
}
