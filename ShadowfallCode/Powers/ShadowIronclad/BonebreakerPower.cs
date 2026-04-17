using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public class BonebreakerPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Bonebreaker>();
    protected override bool IsPositive => false;
}