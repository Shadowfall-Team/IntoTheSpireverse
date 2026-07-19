using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;

public class FireEverythingPower : ShadowPowerModel, IModifiesShotCost
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;

    public int ModifyShotCost(int current) => 0;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            Flash();
            await PowerCmd.Remove(this);
        }
    }
}