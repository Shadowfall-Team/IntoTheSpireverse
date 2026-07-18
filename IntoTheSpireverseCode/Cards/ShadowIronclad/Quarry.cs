using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless.Rocks;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Quarry() : ShadowIroncladCard(-1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<SmallRock>(IsUpgraded),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var x = ResolveEnergyXValue();

        if (x > 0)
        {
            await CreatureCmdCompatibility.Damage(choiceContext, Owner.Creature,
                x, ValueProp.Unblockable | ValueProp.Unpowered, this, cardPlay);
        }

        var count = x + 1;
        var rocks = new CardModel[count];

        for (var i = 0; i < count; i++)
        {
            rocks[i] = CombatState.CreateCard<SmallRock>(Owner);
            if (IsUpgraded)
                CardCmd.Upgrade(rocks[i]);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(rocks, PileType.Hand, Owner);
    }
}