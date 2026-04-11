using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Relics.ShadowRegent;

//TODO needs name
public class SpareBullet() : ShadowRegentRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<AmmoPower>(1)
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            await PowerCmd.Apply<AmmoPower>(Owner.Creature,
                DynamicVars[nameof(AmmoPower)].BaseValue, Owner.Creature, null);
        }
    }

    public override RelicModel? GetUpgradeReplacement()
    {
        return ModelDb.Relic<Bandolier>();
    }
}