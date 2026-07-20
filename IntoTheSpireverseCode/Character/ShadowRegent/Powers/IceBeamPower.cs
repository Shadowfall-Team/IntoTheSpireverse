using BaseLib.Abstracts;
using BaseLib.Extensions;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;

public class IceBeamPower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<IceBeam>();

    string? ICustomPower.CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    string? ICustomPower.CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    protected override bool IsPositive => false;
}