using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// Foothold's "Strength this turn".
///
/// The base game's <see cref="TemporaryStrengthPower"/> is a base class, not a registered model, so
/// referencing it through ModelDb - which HoverTipFactory.FromPower and PowerVar both do - throws
/// KeyNotFoundException for POWER.TEMPORARY_STRENGTH_POWER. Every temporary-Strength effect in this
/// mod therefore has its own registered wrapper; this is Foothold's.
/// </summary>
public class FootholdTemporaryStrengthPower : CustomTemporaryPowerModelWrapper<Foothold, StrengthPower>;
