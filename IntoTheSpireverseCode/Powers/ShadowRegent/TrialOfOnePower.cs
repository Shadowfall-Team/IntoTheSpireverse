using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class TrialOfOnePower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("EnergySpentThisTurn", 0)
    ];
    
    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side)
        {
            return Task.CompletedTask;
        }

        DynamicVars["EnergySpentThisTurn"].BaseValue = 0;
        StopPulsing();
        return Task.CompletedTask;
    }

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature == Owner)
        {
            if (CombatManager.Instance.IsInProgress && amount > 0)
            {
                DynamicVars["EnergySpentThisTurn"].BaseValue += amount;
                if (DynamicVars["EnergySpentThisTurn"].BaseValue % 4 == 0)
                {
                    StartPulsing();
                }

                if (DynamicVars["EnergySpentThisTurn"].BaseValue > 4)
                {
                    Flash();

                    await PowerCmd.Apply<StrengthPower>(
                        new ThrowingPlayerChoiceContext(),
                        Owner,
                        Amount,
                        Owner,
                        null);
                    
                    await PowerCmd.Apply<IntangiblePower>(
                        new ThrowingPlayerChoiceContext(),
                        Owner,
                        1,
                        Owner,
                        null);
                    
                    await PowerCmd.Remove(this);
                }
            }
        }
    }
}