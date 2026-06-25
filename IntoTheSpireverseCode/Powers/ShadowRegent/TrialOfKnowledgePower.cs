using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class TrialOfKnowledgePower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (Owner.Player == null) return;
        if (side == CombatSide.Enemy)
            return;

        if (PileType.Hand.GetPile(Owner.Player).Cards.Count == 5)
        {
            Flash();

            await PowerCmd.Apply<ShardsPower>(
                new ThrowingPlayerChoiceContext(),
                Owner,
                Amount,
                Owner, null);

            await PowerCmd.Remove(this);
        }
    }
}