using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowIronclad;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowIronclad;

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
        await CreatureCmd.Damage(choiceContext, Owner, Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, target, null);

        var relic = target.Player?.Relics.OfType<Buckler>().FirstOrDefault();
        if (relic != null)
            await relic.TryHeal();
    }
}