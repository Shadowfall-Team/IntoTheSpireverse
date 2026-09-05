using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// Flex's "Strength this turn". See <see cref="FootholdTemporaryStrengthPower"/> for why each
/// temporary-Strength effect needs its own registered wrapper rather than sharing one.
/// </summary>
public class FlexTemporaryStrengthPower : CustomTemporaryPowerModelWrapper<Flex, StrengthPower>;
