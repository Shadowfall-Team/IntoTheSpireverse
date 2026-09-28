using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards.Rocks;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// InvertInternalPowerAmount makes this a Strength loss, like the base game's PiercingWailPower:
/// callers pass a positive amount.
/// </summary>
public class GhostRockTemporaryStrengthPower : CustomTemporaryPowerModelWrapper<GhostRock, StrengthPower>
{
    protected override bool InvertInternalPowerAmount => true;
}
