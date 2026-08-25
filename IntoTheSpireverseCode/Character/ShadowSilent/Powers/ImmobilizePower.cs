using BaseLib.Abstracts;
using BaseLib.Extensions;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class ImmobilizePower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Immobilize>();
    protected override bool IsPositive => false;
    
    string? ICustomPower.CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    string? ICustomPower.CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}