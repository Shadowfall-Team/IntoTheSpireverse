using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class ShipMaintenance() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        if (CombatState != null)
        {
            var automation = CombatState.CreateCard<Automation>(Owner);
            automation.AddKeyword(CardKeyword.Ethereal);
            var prowess = CombatState.CreateCard<Prowess>(Owner);
            prowess.AddKeyword(CardKeyword.Ethereal);
            var stratagem = CombatState.CreateCard<Stratagem>(Owner);
            stratagem.AddKeyword(CardKeyword.Ethereal);

            await CardPileCmd.Add([automation, prowess, stratagem], PileType.Hand,
                source: this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}