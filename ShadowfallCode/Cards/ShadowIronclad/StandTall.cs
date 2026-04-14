using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Keywords;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class StandTall() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const string ResolveKey = "ResolveAmount";

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(10m, ValueProp.Move),
        new ShadowfallKeywords.GloryVar(3m),
        new DynamicVar(ResolveKey, 5m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ShadowfallKeywords.GloryHoverTipDynamic(DynamicVars[ShadowfallKeywords.GloryVar.defaultName]),
        HoverTipFactory.FromPower<ResolvePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        if (ShadowfallKeywords.IsGloryTriggered(this))
            await PowerCmd.Apply<ResolvePower>(Owner.Creature, DynamicVars[ResolveKey].BaseValue, Owner.Creature, (CardModel?)null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars[ResolveKey].UpgradeValueBy(1m);
    }
}