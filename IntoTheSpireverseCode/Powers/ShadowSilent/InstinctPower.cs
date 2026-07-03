using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;

public class InstinctPower : IntoTheSpireversePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
