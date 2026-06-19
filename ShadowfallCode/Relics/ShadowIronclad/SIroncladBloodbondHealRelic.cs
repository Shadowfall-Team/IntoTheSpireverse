using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Relics.ShadowIronclad;

public class Buckler : ShadowIroncladRelic
{
    private bool _activatedThisCombat;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(4m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BloodbondPower>(),
    ];

    private bool ActivatedThisCombat
    {
        get => _activatedThisCombat;
        set
        {
            AssertMutable();
            _activatedThisCombat = value;
        }
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom) return Task.CompletedTask;

        ActivatedThisCombat = false;
        return Task.CompletedTask;
    }

    public async Task TryHeal()
    {
        if (ActivatedThisCombat) return;
        if (Owner.Creature.IsDead) return;

        ActivatedThisCombat = true;
        Flash();
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
}