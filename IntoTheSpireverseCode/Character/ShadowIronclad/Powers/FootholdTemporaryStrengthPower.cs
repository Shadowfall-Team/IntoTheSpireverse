using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// <see cref="TemporaryStrengthPower"/> is abstract and unregistered, so ModelDb lookups
/// (HoverTipFactory.FromPower, PowerVar) throw on it. Each temporary-Strength effect gets its own
/// registered wrapper instead.
/// </summary>
public class FootholdTemporaryStrengthPower : CustomTemporaryPowerModelWrapper<Foothold, StrengthPower>;
