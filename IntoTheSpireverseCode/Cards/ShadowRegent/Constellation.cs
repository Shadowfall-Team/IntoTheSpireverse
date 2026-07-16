using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.utils;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

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

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);

        var hand = PileType.Hand.GetPile(Owner)
            .Cards.ToList();
        var drawPile = PileType.Draw.GetPile(Owner).Cards
            .OrderBy(c => c.Rarity)
            .ThenBy(c => c.Id).ToList();
        List<CardModel> drawOrHand = [..hand, ..drawPile];
        if (drawOrHand.Count == 0) return;

        var noChoice = drawOrHand.Count == 1;

        var cardSelectorPrefs =
            new CardSelectorPrefs(new LocString("card_selection", "INTOTHESPIREVERSE-CONSTELLATION_SELECT"), 1);
        var results =
            (await CardSelectCmd.FromSimpleGrid(choiceContext, drawOrHand, Owner,
                cardSelectorPrefs)).ToList();

        if (noChoice)
        {
            await CardPileCmdExtras.TransferPileAndPreview(results, results.First().Pile!.Type, CargoCardPile.CargoPileType);
        }
        else
        {
            await CardPileCmd.Add(results, CargoCardPile.CargoPileType);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}