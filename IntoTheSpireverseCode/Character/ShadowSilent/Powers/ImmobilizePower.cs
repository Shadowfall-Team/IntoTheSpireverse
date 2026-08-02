using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class ImmobilizePower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Immobilize>();
    protected override bool IsPositive => false;
}