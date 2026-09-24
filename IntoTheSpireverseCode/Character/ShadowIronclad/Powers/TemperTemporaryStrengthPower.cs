using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// Temper's "Temporary Strength". See <see cref="FootholdTemporaryStrengthPower"/> for why each
/// temporary-Strength effect needs its own registered wrapper.
/// </summary>
public class TemperTemporaryStrengthPower : CustomTemporaryPowerModelWrapper<Temper, StrengthPower>;
