using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowNecrobinder;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowNecrobinder;

public class TimeOutPower : CustomTemporaryPowerModelWrapper<TimeOut, StrengthPower>
{
    protected override bool InvertInternalPowerAmount => true;
}