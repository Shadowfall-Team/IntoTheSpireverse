using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class TargetedPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
}