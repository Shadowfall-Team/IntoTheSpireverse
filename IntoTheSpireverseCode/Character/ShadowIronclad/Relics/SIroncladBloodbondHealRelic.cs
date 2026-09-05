using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Relics;

public class Buckler : ShadowIroncladRelic
{
    private bool _activatedThisCombat;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(4m),
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

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext,
        Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner.Creature || result.UnblockedDamage <= 0) return;
        if (Owner.Creature.CombatState?.CurrentSide != Owner.Creature.Side) return;
        if (ActivatedThisCombat || Owner.Creature.IsDead) return;

        ActivatedThisCombat = true;
        Flash();
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }
}
