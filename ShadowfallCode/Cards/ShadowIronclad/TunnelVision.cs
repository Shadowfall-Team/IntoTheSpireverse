using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class TunnelVision() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ShadowfallKeywords.GloryVar(2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ShadowfallKeywords.GloryHoverTipDynamic(DynamicVars[ShadowfallKeywords.GloryVar.defaultName]),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this)).FirstOrDefault();
        if (card != null)
            await CardCmd.Exhaust(choiceContext, card);

        var strikes = Owner.UnlockState.CharacterCardPools
            .SelectMany(pool => pool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint))
            .Where(c => c.Tags.Contains(CardTag.Strike))
            .ToList();

        var strike = CardFactory.GetDistinctForCombat(Owner, strikes, 1, Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();

        if (strike == null) return;

        if (ShadowfallKeywords.IsGloryTriggered(this))
            strike.EnergyCost.AddThisCombat(-1);

        await CardPileCmd.AddGeneratedCardToCombat(strike, PileType.Hand, true);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}