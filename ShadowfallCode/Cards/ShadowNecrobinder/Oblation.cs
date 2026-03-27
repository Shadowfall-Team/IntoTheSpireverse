using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class Oblation() : ShadowNecrobinderCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Ethereal
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SoulStrike>(IsUpgraded)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var drawPile = PileType.Draw.GetPile(Owner).Cards
            .OrderBy(c => c.Rarity)
            .ThenBy(c => c.Id)
            .ToList();

        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, drawPile, Owner,
            new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, DynamicVars.Cards.IntValue));

        foreach (var original in selected.ToList())
        {
            CardPileAddResult? result = await CardCmd.TransformTo<SoulStrike>(original);
            if (IsUpgraded && result.HasValue)
                CardCmd.Upgrade(result.Value.cardAdded);
        }
    }
}