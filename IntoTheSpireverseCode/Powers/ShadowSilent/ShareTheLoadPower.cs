using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;

public sealed class ShareTheLoadPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        Flash();
        IEnumerable<Creature> players = from c in base.CombatState.GetTeammatesOf(base.Owner)
            where c is { IsAlive: true, IsPlayer: true }
            select c;
        
        foreach (var player in players)
        {
            await CreatureCmd.GainBlock(player, Amount, ValueProp.Unpowered, null);
        }
    }
}
