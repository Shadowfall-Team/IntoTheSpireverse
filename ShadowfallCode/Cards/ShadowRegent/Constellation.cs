using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.CardPiles;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class Constellation() : ShadowRegentCard(
    0,
    CardType.Skill,
    CardRarity.Ancient,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8, ValueProp.Move)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Cargo)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        var drawPile = PileType.Draw.GetPile(Owner)
            .Cards.OrderBy(c => c.Rarity)
            .ThenBy(c => c.Id).ToList();
        if (drawPile.Count == 0) return;
        
        var cardSelectorPrefs =
            new CardSelectorPrefs(CargoSelectorPrefs.ToCargoSelectionPrompt, 1);
        var results =
            (await CardSelectCmd.FromSimpleGrid(choiceContext, drawPile, Owner,
                cardSelectorPrefs)).ToList();

        //TODO: check all cardpilecmd add/previews if they no longer need to skip visuals after beta is merged into main
        await CardPileCmd.Add(results, CargoCardPile.CargoPileType, skipVisuals: true);
        CardCmd.Preview(results);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}