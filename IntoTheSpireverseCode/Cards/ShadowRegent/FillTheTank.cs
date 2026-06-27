using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class FillTheTank() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo),
        IsUpgraded ? HoverTipFactory.FromCard<Fuel>(true) : HoverTipFactory.FromCard<Fuel>()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState != null)
        {
            var fuelCard = CombatState.CreateCard<Fuel>(Owner);
            if (IsUpgraded)
            {
                CardCmd.Upgrade(fuelCard);
            }
            var cardAdd = await CardPileCmd.AddGeneratedCardToCombat(fuelCard, CargoCardPile.CargoPileType, Owner);
            CardCmd.PreviewCardPileAdd(cardAdd);
            
            //TODO - This probably doesn't work or could be written better.
            cardAdd = await CardPileCmd.AddGeneratedCardToCombat(fuelCard, CargoCardPile.CargoPileType, Owner);
            CardCmd.PreviewCardPileAdd(cardAdd);

        }
    }

    protected override void OnUpgrade()
    {
        
    }
}