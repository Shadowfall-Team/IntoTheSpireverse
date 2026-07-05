using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowIronclad;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowIronclad;

public class LandslidePower : CustomTemporaryPowerModelWrapper<Landslide, StrengthPower>
{
    protected override bool InvertInternalPowerAmount => true;
}