using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public class SuperheatedTemporaryStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Superheated>();
}