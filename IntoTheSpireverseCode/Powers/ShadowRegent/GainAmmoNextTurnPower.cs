using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

public class GainAmmoNextTurnPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        await LoadAmmoCmd.LoadAmmo(Amount, Owner.Player);
        await PowerCmd.Remove(this);
    }
}