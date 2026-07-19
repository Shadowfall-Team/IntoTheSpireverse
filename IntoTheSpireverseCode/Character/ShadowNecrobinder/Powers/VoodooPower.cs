using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowNecrobinder.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowNecrobinder.Powers;

public class VoodooPower : CustomTemporaryPowerModelWrapper<Voodoo, StrengthPower>
{
    protected override bool InvertInternalPowerAmount => true;
}