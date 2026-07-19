using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class ShankAndFlankPower : IntoTheSpireversePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState) 
    {
        if (side != Owner.Side || Owner.Player == null) return;

        Flash();
        for (int i = 0; i < Amount; i++)
        {
            await Shiv.CreateInHand(Owner.Player, 1, CombatState);
            await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Ward>(Owner.Player), PileType.Hand, Owner.Player);
        }
    }
}
