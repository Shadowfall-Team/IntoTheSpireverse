using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class TrialOfKnowledge() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<TrialOfKnowledgePower>(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        if (IsUpgraded)
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }

        await PowerCmd.Apply<TrialOfKnowledgePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(TrialOfKnowledgePower)].BaseValue,
            Owner.Creature,
            this);
    }
}