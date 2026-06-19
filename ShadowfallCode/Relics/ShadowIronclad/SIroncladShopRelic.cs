using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Relics.ShadowIronclad;

public class CrimsonAmulet : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BloodbondPower>(5m),
        new PowerVar<ThornsPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BloodbondPower>(),
        HoverTipFactory.FromPower<ThornsPower>(),
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom) return;

        var targets = Owner.Creature.CombatState
            .GetOpponentsOf(Owner.Creature)
            .Where(c => c.IsAlive);

        Flash();

        await PowerCmd.Apply<BloodbondPower>(
            new ThrowingPlayerChoiceContext(),
            targets, DynamicVars.Power<BloodbondPower>().BaseValue,
            null, null);

        await PowerCmd.Apply<ThornsPower>(
            new ThrowingPlayerChoiceContext(),
            targets, DynamicVars.Power<ThornsPower>().BaseValue,
            null, null);
    }

    public override async Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (creature.Side == Owner.Creature.Side) return;

        Flash();

        await PowerCmd.Apply<BloodbondPower>(
            new ThrowingPlayerChoiceContext(),
            creature, DynamicVars.Power<BloodbondPower>().BaseValue,
            null, null);

        await PowerCmd.Apply<ThornsPower>(
            new ThrowingPlayerChoiceContext(),
            creature, DynamicVars.Power<ThornsPower>().BaseValue,
            null, null);
    }
}