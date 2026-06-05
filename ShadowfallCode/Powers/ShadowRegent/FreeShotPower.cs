using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Ammo;
using Shadowfall.ShadowfallCode.Cards.Colorless;

namespace Shadowfall.ShadowfallCode.Powers.ShadowRegent;

public class FreeShotPower : CustomPowerModel, IAmmoFiredListener
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

    public async void OnAmmoFired(Player player, IReadOnlyList<Creature> targets)
    {
        if (player.Creature != Owner) return;
        Flash();
        await PowerCmd.Decrement(this);
    }
}
