using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class FillTheTank() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo),
        HoverTipFactory.FromCard<Fuel>(IsUpgraded)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var fuelCard = CombatState!.CreateCard<Fuel>(Owner);
            if (IsUpgraded)
            {
                CardCmd.Upgrade(fuelCard);
            }

            var cardAdd = await CardPileCmd.AddGeneratedCardToCombat(fuelCard, CargoCardPile.CargoPileType, Owner);
            CardCmd.PreviewCardPileAdd(cardAdd);
        }
    }
}