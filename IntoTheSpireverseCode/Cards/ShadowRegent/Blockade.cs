using BaseLib.Extensions;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Blockade() : ShadowRegentCard(2,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VolleyDamagePower>(4),
        new BlockVar(2, ValueProp.Unpowered)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<VolleyDamagePower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars.Power<VolleyDamagePower>().BaseValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<BlockadePower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars.Block.BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1);
        DynamicVars.Power<VolleyDamagePower>().UpgradeValueBy(2);
    }
}