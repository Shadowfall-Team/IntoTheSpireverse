using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class CarefulPlanning() : ShadowSilentCard(-1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable, CardKeyword.Ethereal];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(2m, ValueProp.Move),
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && ShadowfallKeywords.WasAdjacentWhenRemoved(cardPlay.Card, this))
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null, false);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}
