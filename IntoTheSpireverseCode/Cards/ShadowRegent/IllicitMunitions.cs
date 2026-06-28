using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class IllicitMunitions() : ShadowRegentCard(0,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo),
        ..HoverTipFactory.FromEnchantment<Steady>(),
        HoverTipFactory.FromCard<Volley>(IsUpgraded),
        HoverTipFactory.FromCard<Salvo>(IsUpgraded)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState == null) return;

        var volleyCard = CombatState.CreateCard<Volley>(Owner);
        CardCmd.Enchant<Steady>(volleyCard, 1);

        var salvoCard = CombatState.CreateCard<Salvo>(Owner);
        CardCmd.Enchant<Steady>(salvoCard, 1);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(volleyCard);
            CardCmd.Upgrade(salvoCard);
        }

        var results = await CardPileCmd.AddGeneratedCardsToCombat([volleyCard, salvoCard], CargoCardPile.CargoPileType,
            Owner);

        CardCmd.PreviewCardPileAdd(results);
    }

    protected override void OnUpgrade()
    {
    }
}