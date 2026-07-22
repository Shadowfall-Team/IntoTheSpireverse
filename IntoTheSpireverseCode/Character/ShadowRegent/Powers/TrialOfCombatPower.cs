using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;

public class TrialOfCombatPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("AttacksPlayedThisTurn", 0)
    ];

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
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
        if (Owner.Player == null || cardPlay.Card.Owner.Creature != Owner || !CombatManager.Instance.IsInProgress ||
            cardPlay.Card.Type != CardType.Attack) return;

        DynamicVars["AttacksPlayedThisTurn"].BaseValue++;

        // The power is removed the moment it fires, so the counter never wraps and these read
        // as "at 3" and "at 4" - one attack of warning, then the payoff.
        if (DynamicVars["AttacksPlayedThisTurn"].BaseValue % 3 == 0)
        {
            StartPulsing();
        }

        if (DynamicVars["AttacksPlayedThisTurn"].BaseValue % 4 == 0)
        {
            Flash();

            for (int i = 0; i < Amount; i++)
            {
                var sacCard = CombatState.CreateCard<MinionSacrifice>(Owner.Player);
                CardCmd.Enchant<Steady>(sacCard, 1);
                await CardPileCmd.AddGeneratedCardToCombat(sacCard, PileType.Hand, Owner.Player);
            }

            await PowerCmd.Remove(this);
        }
    }
}