using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Relics.ShadowIronclad;

public class HeartOfTheMountain : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("HealPercent", 15m),
        new MaxHpVar(15m),
    ];

    public override async Task AfterObtained()
    {
        Flash();
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (room.RoomType != RoomType.Elite) return;
        if (Owner.Creature.IsDead) return;

        Flash();
        await CreatureCmd.Heal(Owner.Creature, Owner.Creature.MaxHp * DynamicVars["HealPercent"].BaseValue / 100m);
    }
}