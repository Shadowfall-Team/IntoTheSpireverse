using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Shadowfall.ShadowfallCode.Commands;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Relics.ShadowRegent;

//TODO needs name
public class SpareBullet() : ShadowRegentRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("LoadAmmo", 1)
    ];

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side == Owner.Creature.Side)
        {
            if (combatState.RoundNumber <= 1)
            {
                await LoadAmmoCmd.LoadAmmo(1, Owner, this);
            }
        }
    }

    public override RelicModel? GetUpgradeReplacement()
    {
        return ModelDb.Relic<Bandolier>();
    }
}