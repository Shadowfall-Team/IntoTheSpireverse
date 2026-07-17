using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Mayday() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo),
        HoverTipFactory.FromCard<PanicButton>(IsUpgraded),
        HoverTipFactory.FromCard<Shockwave>(IsUpgraded),
        HoverTipFactory.FromCard<Catastrophe>(IsUpgraded)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        // Panic Button to hand
        var panicCard = CombatState.CreateCard<PanicButton>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(panicCard);
        }
        await CardPileCmd.AddGeneratedCardToCombat(panicCard, PileType.Hand, Owner);

        //Shockwave and Catastrophe to Cargo
        var shockCard = CombatState.CreateCard<Shockwave>(Owner);

        var cataCard = CombatState.CreateCard<Catastrophe>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(shockCard);
            CardCmd.Upgrade(cataCard);
        }

        var results = await CardPileCmd.AddGeneratedCardsToCombat([shockCard, cataCard], CargoCardPile.CargoPileType,
            Owner);

        CardCmd.PreviewCardPileAdd(results);
    }

    protected override void OnUpgrade()
    {
    }
}
