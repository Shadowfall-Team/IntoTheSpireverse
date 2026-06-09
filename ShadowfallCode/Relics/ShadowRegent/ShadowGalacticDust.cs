using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards.Colorless;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Relics.ShadowRegent;

public class ShadowGalacticDust : ShadowRegentRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5, ValueProp.Unpowered)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Warp>()
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext context,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.Card is Warp)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }
    }
}