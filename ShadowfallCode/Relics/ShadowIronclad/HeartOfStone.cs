using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Relics.ShadowIronclad;

public class HeartOfStone : ShadowIroncladRelic
{
    private int _hpLost;

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
    public override int DisplayAmount => _hpLost;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(10m),
    ];
    
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<HeartOfTheMountain>();

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner.Creature) return;
        if (Owner.Creature.CombatState?.CurrentSide != Owner.Creature.Side) return;
        if (result.UnblockedDamage <= 0) return;
        _hpLost = Math.Min(_hpLost + result.UnblockedDamage, (int)DynamicVars.Heal.BaseValue);
        InvokeDisplayAmountChanged();
    }

    public override async Task AfterCombatVictory(CombatRoom _)
    {
        if (Owner.Creature.IsDead || _hpLost <= 0) return;
        Flash();
        await CreatureCmd.Heal(Owner.Creature, (decimal)_hpLost);
        _hpLost = 0;
        InvokeDisplayAmountChanged();
    }
}