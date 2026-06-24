using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowNecrobinder;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowNecrobinder;

public class TimeOutPower : TemporaryStrengthPower
{
    // Power format may be incorrect, unsure if BaseLib has or is going to have a CustomPowerModel equivalent for Temp. Strength effects
    public override AbstractModel OriginModel => ModelDb.Card<TimeOut>();
    protected override bool IsPositive => false;
}