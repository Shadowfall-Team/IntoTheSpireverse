using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards.Statuses;

[Pool(typeof(StatusCardPool))]
public sealed class Mud() : IntoTheSpireverseCard(-1, CardType.Status, CardRarity.Status, TargetType.None, "ironclad"),
    ITransformPayout
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

    public bool WaitsForCardPlay => true;

    public async Task OnTransformedAway(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<SlatePower>(
            choiceContext,
            Owner.Creature, DynamicVars.Power<SlatePower>().BaseValue,
            Owner.Creature, null);
    }
}
