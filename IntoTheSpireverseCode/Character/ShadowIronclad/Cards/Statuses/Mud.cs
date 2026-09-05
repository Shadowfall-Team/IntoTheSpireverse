using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards.Statuses;

/// <summary>
/// The Slate grant is applied by <see cref="Patches.TransformPayoutPatches"/>, because
/// <see cref="MegaCrit.Sts2.Core.Models.CardModel.AfterTransformedFrom"/> is synchronous and
/// power application is not.
/// </summary>
[Pool(typeof(StatusCardPool))]
public sealed class Mud() : IntoTheSpireverseCard(-1, CardType.Status, CardRarity.Status, TargetType.None, "ironclad")
{
    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Unplayable,
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SlatePower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SlatePower>(),
    ];
}
