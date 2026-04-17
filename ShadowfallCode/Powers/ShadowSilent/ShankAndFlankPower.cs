using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Cards;
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public class ShankAndFlankPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side) return;

        Flash();
        for (int i = 0; i < Amount; i++)
        {
            await Shiv.CreateInHand(Owner.Player, 1, CombatState);
            await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Ward>(Owner.Player), PileType.Hand, Owner.Player);
        }
    }
}
