using BaseLib.Extensions;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Relics;

public class CrimsonAmulet : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(10m),
        new PowerVar<ThornsPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ThornsPower>(),
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom) return;

        var targets = Owner.Creature.CombatState?
            .GetOpponentsOf(Owner.Creature)
            .Where(c => c.IsAlive).ToList();

        Flash();

        await PowerCmd.Apply<ThornsPower>(
            new ThrowingPlayerChoiceContext(),
            targets, DynamicVars.Power<ThornsPower>().BaseValue,
            null, null);
    }

    // Done by the relic rather than by applying Bloodbond, so enemies summoned mid-combat are hit too.
    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner.Creature) return;
        if (target.CombatState?.CurrentSide != target.Side) return;
        if (result.UnblockedDamage <= 0) return;

        var enemies = target.CombatState
            .GetOpponentsOf(target)
            .Where(c => c.IsAlive).ToList();
        if (enemies.Count == 0) return;

        Flash();
        await CreatureCmdCompatibility.Damage(choiceContext, enemies, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered, target, null, null);
    }
}
