using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class GarbageDay() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo),
        HoverTipFactory.FromCard<Debris>(),
        HoverTipFactory.FromCard<Bury>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await CardPileCmd.AddToCombatAndPreview<Debris>(Owner.Creature,
            CargoCardPile.CargoPileType, 4, Owner);

        var bury = CombatState.CreateCard<Bury>(Owner);
        bury.EnergyCost.SetThisCombat(0);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(bury);
        }

        var cardPileAddResult = await CardPileCmd.AddGeneratedCardToCombat(bury, CargoCardPile.CargoPileType, Owner);
        CardCmd.PreviewCardPileAdd(cardPileAddResult);
    }
}