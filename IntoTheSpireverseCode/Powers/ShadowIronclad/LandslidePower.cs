using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowIronclad;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowIronclad;

public class LandslidePower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Landslide>();
    protected override bool IsPositive => false;
}