using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Relics;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public class BloodbondPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    // Make this instanced a la GrapplePower to have this not be affected by other players
    // But then it doesn't stack and looks all weird


    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult damageResult,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target.Side != CombatSide.Player) return;
        if (CombatState?.CurrentSide != target.Side) return;
        if (damageResult.UnblockedDamage <= 0) return;

        Flash();
        await CreatureCmdCompatibility.Damage(choiceContext, Owner, Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, target, null, null);

        var relic = target.Player?.Relics.OfType<Buckler>().FirstOrDefault();
        if (relic != null)
            await relic.TryHeal();
    }
}