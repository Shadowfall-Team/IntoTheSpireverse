using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class ShipMaintenance() : ShadowRegentCard(
    0,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var automation = (CardModel)ModelDb.Card<Automation>().MutableClone();
            automation.AddKeyword(CardKeyword.Ethereal);
            var prowess = (CardModel)ModelDb.Card<Prowess>().MutableClone();
            prowess.AddKeyword(CardKeyword.Ethereal);
            var stratagem = (CardModel)ModelDb.Card<Stratagem>().MutableClone();
            stratagem.AddKeyword(CardKeyword.Ethereal);
            return
            [
                HoverTipFactory.FromCard(automation),
                HoverTipFactory.FromCard(prowess),
                HoverTipFactory.FromCard(stratagem),
                HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
            ];
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        if (IsUpgraded)
        {
            await PlayerCmd.GainEnergy(1, Owner);
        }

        if (CombatState != null)
        {
            var automation = CombatState.CreateCard<Automation>(Owner);
            automation.AddKeyword(CardKeyword.Ethereal);
            var prowess = CombatState.CreateCard<Prowess>(Owner);
            prowess.AddKeyword(CardKeyword.Ethereal);
            var stratagem = CombatState.CreateCard<Stratagem>(Owner);
            stratagem.AddKeyword(CardKeyword.Ethereal);

            await CardPileCmd.AddGeneratedCardsToCombat([automation, prowess, stratagem], PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade()
    {
    }
}