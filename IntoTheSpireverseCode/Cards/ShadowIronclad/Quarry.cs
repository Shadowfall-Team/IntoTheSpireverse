using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless.Rocks;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowIronclad;

public sealed class Quarry() : ShadowIroncladCard(-1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SmallRock>(IsUpgraded),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var count = ResolveEnergyXValue() + 1;
        var rocks = new List<CardModel>(count);

        for (var i = 0; i < count; i++)
        {
            var rock = CombatState!.CreateCard<SmallRock>(Owner);
            if (IsUpgraded)
            {
                CardCmd.Upgrade(rock);
            }

            rocks.Add(rock);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(rocks, PileType.Hand, Owner);
    }
}