using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using IntoTheSpireverse.Orbs;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards;

public sealed class Collapse() : ShadowDefectCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        int numOfOrbs = this.ResolveEnergyXValue();
        if (IsUpgraded)
            ++numOfOrbs;

        for (int i = 0; i < numOfOrbs; i++)
            await OrbCmd.Channel<EntropyOrb>(choiceContext, Owner);
    }

    protected override void OnUpgrade() { }
}