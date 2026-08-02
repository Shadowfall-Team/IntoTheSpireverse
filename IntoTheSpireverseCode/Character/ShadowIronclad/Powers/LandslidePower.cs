using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public class LandslidePower : CustomTemporaryPowerModelWrapper<Landslide, StrengthPower>
{
    protected override bool InvertInternalPowerAmount => true;
}