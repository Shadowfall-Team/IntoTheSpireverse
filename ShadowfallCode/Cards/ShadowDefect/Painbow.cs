using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using Shadowfall.Orbs;
using Shadowfall.ShadowfallCode.Cards;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class Painbow() : ShadowDefectCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await OrbCmd.Channel<EntropyOrb>(choiceContext, Owner);
        await OrbCmd.Channel<FrostOrb>(choiceContext, Owner);
        await OrbCmd.Channel<DarkOrb>(choiceContext, Owner);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}

