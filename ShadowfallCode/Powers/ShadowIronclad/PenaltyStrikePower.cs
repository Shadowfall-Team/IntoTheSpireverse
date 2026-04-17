using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public class PenaltyStrikePower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<PenaltyStrike>();
    protected override bool IsPositive => false;
}