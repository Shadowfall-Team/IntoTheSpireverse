using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards.Rocks;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// Ghost Rock's Strength loss. InvertInternalPowerAmount is BaseLib's equivalent of the base game's
/// TemporaryStrengthPower.IsPositive = false (see PiercingWailPower): the applied amount is negated
/// and the power reports as a Debuff, so callers pass a positive number as the base game does.
/// </summary>
public class GhostRockTemporaryStrengthPower : CustomTemporaryPowerModelWrapper<GhostRock, StrengthPower>
{
    protected override bool InvertInternalPowerAmount => true;
}
