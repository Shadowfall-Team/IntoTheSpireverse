using BaseLib.Hooks;
using Godot;
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
    private static readonly Color ForecastColor = new("FB8DFF");

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// Forecasts the HP this creature will lose the next time the bonded player takes damage, drawn
    /// doom-style: chained from the empty edge, growing toward current HP.
    ///
    /// Left at the default AffectsHpLabel, so a Bloodbond that covers the whole remaining bar tints
    /// the HP label the way a lethal Doom does.
    /// </summary>
    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(
        HealthBarForecastContext context)
    {
        if (Amount <= 0) yield break;
        if (context.Creature.CurrentHp <= 0) yield break;

        yield return new HealthBarForecastSegment(
            Amount, ForecastColor, HealthBarForecastDirection.FromLeft);
    }

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

        // A Bloodbond on the bonded creature itself would answer its own damage with more damage,
        // re-entering this hook until the creature died. Cannot arise from normal play, where the
        // power sits on an enemy and the trigger is the player being hit.
        if (target == Owner) return;

        Flash();
        await CreatureCmdCompatibility.Damage(choiceContext, Owner, Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, target, null, null);
    }
}