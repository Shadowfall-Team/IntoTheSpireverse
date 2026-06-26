using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

public class EMPulsePower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<EMPulse>();

    protected override bool IsPositive => false;
}