using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

namespace Shadowfall.ShadowfallCode.Powers.ShadowNecrobinder;

public class VoodooPower : TemporaryStrengthPower
{
    // Power format may be incorrect, unsure if BaseLib has or is going to have a CustomPowerModel equivalent for Temp. Strength effects
    public override AbstractModel OriginModel => ModelDb.Card<Voodoo>();
    protected override bool IsPositive => false;
}