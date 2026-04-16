using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class VakuusInsight() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ShadowfallKeywords.GloryVar(2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ShadowfallKeywords.GloryHoverTipDynamic(DynamicVars[ShadowfallKeywords.GloryVar.defaultName]),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var powers = PileType.Draw.GetPile(Owner).Cards
            .Where(c => c.Type == CardType.Power)
            .ToList();

        if (powers.Count == 0) return;

        var card = Owner.RunState.Rng.CombatCardSelection.NextItem(powers);

        if (ShadowfallKeywords.IsGloryTriggered(this))
            card.EnergyCost.SetThisCombat(0);

        await CardPileCmd.Add(card, PileType.Hand);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}