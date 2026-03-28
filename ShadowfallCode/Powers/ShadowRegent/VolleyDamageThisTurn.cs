using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Shadowfall.ShadowfallCode.Powers.ShadowRegent;

public class VolleyDamageThisTurn : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}