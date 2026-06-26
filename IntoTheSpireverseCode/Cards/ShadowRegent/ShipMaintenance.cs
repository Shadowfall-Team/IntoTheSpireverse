using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class ShipMaintenance() : ShadowRegentCard(
    1,
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
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        var automation = CombatState!.CreateCard<Automation>(Owner);
        automation.AddKeyword(CardKeyword.Ethereal);
        var prowess = CombatState.CreateCard<Prowess>(Owner);
        prowess.AddKeyword(CardKeyword.Ethereal);
        var stratagem = CombatState.CreateCard<Stratagem>(Owner);
        stratagem.AddKeyword(CardKeyword.Ethereal);

        await CardPileCmd.AddGeneratedCardsToCombat([automation, prowess, stratagem], PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}