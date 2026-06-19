using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Potions.ShadowRegent;

public class SpaceDustPotion : ShadowRegentPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo)
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var list = CardFactory.GetDistinctForCombat(
            Owner,
            Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                .Select(c => c), 3, Owner.RunState.Rng.CombatCardGeneration).ToList();

        var cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, list, Owner, true);

        if (cardModel != null)
        {
            cardModel.EnergyCost.SetUntilPlayed(0);
            var cardAdd = await CardPileCmd.AddGeneratedCardToCombat(cardModel, CargoCardPile.CargoPileType, Owner);
            CardCmd.PreviewCardPileAdd(cardAdd);
        }
    }
}