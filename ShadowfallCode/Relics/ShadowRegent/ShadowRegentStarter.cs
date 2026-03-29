using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Relics.ShadowRegent;

//TODO needs name
public class ShadowRegentStarter() : ShadowSilentRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<AmmoPower>(1)
    ];
    
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            await PlayerCmd.GainStars(DynamicVars[nameof(AmmoPower)].BaseValue, Owner);
        }
    }
}

[Pool(typeof(ShadowRegentRelicPool))]
public abstract class ShadowSilentRelic : CustomRelicModel;