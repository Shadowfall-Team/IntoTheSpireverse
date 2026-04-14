using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Relics.ShadowIronclad;

public sealed class CorrupterBlood : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ResolvePower>(5m),
        new PowerVar<RegenPower>(5m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ResolvePower>(),
        HoverTipFactory.FromPower<RegenPower>()
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;

        Flash();
        await PowerCmd.Apply<ResolvePower>(Owner.Creature, DynamicVars["ResolvePower"].BaseValue, Owner.Creature, (CardModel?)null);
        await PowerCmd.Apply<RegenPower>(Owner.Creature, DynamicVars["RegenPower"].BaseValue, Owner.Creature, (CardModel?)null);
    }
}