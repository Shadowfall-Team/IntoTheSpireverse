using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class TrialOfWeaponryPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("AttacksPlayedThisTurn", 0)
    ];

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side)
        {
            return Task.CompletedTask;
        }

        DynamicVars["AttacksPlayedThisTurn"].BaseValue = 0;
        StopPulsing();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner)
        {
            if (CombatManager.Instance.IsInProgress)
            {
                if (cardPlay.Card.Type == CardType.Attack)
                {
                    DynamicVars["AttacksPlayedThisTurn"].BaseValue++;
                    if (DynamicVars["AttacksPlayedThisTurn"].BaseValue % 2 == 0)
                    {
                        StartPulsing();
                    }

                    if (DynamicVars["AttacksPlayedThisTurn"].BaseValue % 3 == 0)
                    {
                        Flash();
                        await LoadAmmoCmd.LoadAmmo(Amount, Owner.Player);
                        await PowerCmd.Remove(this);
                    }
                }
            }
        }
    }
}