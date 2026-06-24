using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers;

public sealed class RodOfRuinPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
 
    public override PowerStackType StackType => PowerStackType.Counter;
 
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromOrb<DarkOrb>()
    };
 
    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player)
            return;
 
        await OrbCmd.Channel<DarkOrb>(new ThrowingPlayerChoiceContext(), Owner.Player);
        await PowerCmd.Decrement(this);
    }
}
