using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Shadowfall.ShadowfallCode.Relics.ShadowIronclad;

public class IronCestus : ShadowIroncladRelic
{
    private bool _isActivating;
    private int _attacksPlayedThisTurn;

    public override RelicRarity Rarity => RelicRarity.Rare;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;

    public override int DisplayAmount =>
        !IsActivating ? AttacksPlayedThisTurn % DynamicVars.Cards.IntValue : DynamicVars.Cards.IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new PowerVar<BlurPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BlurPower>(),
    ];

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            UpdateDisplay();
        }
    }

    private int AttacksPlayedThisTurn
    {
        get => _attacksPlayedThisTurn;
        set
        {
            AssertMutable();
            _attacksPlayedThisTurn = value;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (IsActivating)
        {
            Status = RelicStatus.Normal;
        }
        else
        {
            var threshold = DynamicVars.Cards.IntValue;
            Status = AttacksPlayedThisTurn % threshold == threshold - 1
                ? RelicStatus.Active
                : RelicStatus.Normal;
        }

        InvokeDisplayAmountChanged();
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Creature.Side) return Task.CompletedTask;

        AttacksPlayedThisTurn = 0;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner) return;
        if (!CombatManager.Instance.IsInProgress) return;
        if (cardPlay.Card.Type != CardType.Attack) return;

        AttacksPlayedThisTurn++;

        if (AttacksPlayedThisTurn % DynamicVars.Cards.IntValue != 0) return;

        TaskHelper.RunSafely(DoActivateVisuals());
        await PowerCmd.Apply<BlurPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars.Power<BlurPower>().BaseValue,
            Owner.Creature, null);
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        AttacksPlayedThisTurn = 0;
        IsActivating = false;
        return Task.CompletedTask;
    }
}