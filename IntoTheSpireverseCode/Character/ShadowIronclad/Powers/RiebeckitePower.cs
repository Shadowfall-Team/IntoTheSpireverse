using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public sealed class RiebeckitePower : ShadowPowerModel
{
    public const string RetaliationKey = "Retaliation";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>Amount is the extra Block; the Retaliation var is separate. Both are read by SlatePower.</summary>
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar(RetaliationKey, 1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SlatePower>(),
        HoverTipFactory.FromPower<RetaliationPower>(),
    ];
}