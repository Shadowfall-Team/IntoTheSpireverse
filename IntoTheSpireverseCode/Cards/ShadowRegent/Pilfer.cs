using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Hooks;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Pilfer() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        
        var cargoPile = CargoCardPile.CargoPileType.GetPile(Owner)
            .Cards.OrderBy(c => c.Rarity)
            .ThenBy(c => c.Id).ToList();
        var prefs = new CardSelectorPrefs(CargoSelectorPrefs.FromCargoSelectionPrompt, DynamicVars.Cards.IntValue);

        var selections = await CardSelectCmd.FromSimpleGrid(choiceContext, cargoPile, Owner, prefs);

        if (selections == null || !selections.Any()) return;

        foreach (var selection in selections)
        {
            await CardPileCmd.Add(selection, PileType.Hand);
            await Hook.AfterCardDrawn(CombatState, choiceContext, selection, false);
        }
       }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}