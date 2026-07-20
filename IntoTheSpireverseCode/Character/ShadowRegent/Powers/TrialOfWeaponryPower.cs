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

public class TrialOfWeaponryPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("SkillsPlayedThisTurn", 0)
    ];

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side)
        {
            return Task.CompletedTask;
        }

        DynamicVars["SkillsPlayedThisTurn"].BaseValue = 0;
        StopPulsing();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context,
        CardPlay cardPlay)
    {
        if (Owner.Player == null || cardPlay.Card.Owner.Creature != Owner || CombatManager.Instance.IsInProgress ||
            cardPlay.Card.Type != CardType.Skill) return;
        {
            DynamicVars["SkillsPlayedThisTurn"].BaseValue++;
            if (DynamicVars["SkillsPlayedThisTurn"].BaseValue % 2 == 0)
            {
                StartPulsing();
            }

            if (DynamicVars["SkillsPlayedThisTurn"].BaseValue % 3 == 0)
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
}