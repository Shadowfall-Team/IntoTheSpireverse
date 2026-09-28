using MegaCrit.Sts2.Core.Animation;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.Enchantments;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class ShipMaintenance() : ShadowRegentCard(
    0,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Automation>(),
        HoverTipFactory.FromCard<Prowess>(),
        HoverTipFactory.FromCard<Stratagem>(),
        .. HoverTipFactory.FromEnchantment<Hollow>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger,
            Owner.Character.CastAnimDelay);

        if (IsUpgraded)
        {
            await PlayerCmd.GainEnergy(1, Owner);
        }

        if (CombatState != null)
        {
            CardModel[] cards =
            [
                CombatState.CreateCard<Automation>(Owner),
                CombatState.CreateCard<Prowess>(Owner),
                CombatState.CreateCard<Stratagem>(Owner),
            ];
            foreach (var card in cards)
                CardCmd.Enchant<Hollow>(card, 1m);

            await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade()
    {
    }
}
