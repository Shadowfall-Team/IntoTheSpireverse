using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

public class FreeShotPower : ShadowPowerModel, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card is AmmoVolley)
        {
            modifiedCost = 0;
            return true;
        }
        modifiedCost = originalCost;
        return false;
    }

    public async Task OnAmmoFired(Player player, IEnumerable<List<DamageResult>> results)
    {
        if (player.Creature != Owner) return;
        Flash();
        await PowerCmd.Decrement(this);
    }
}
