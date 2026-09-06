using MegaCrit.Sts2.Core.Animation;
using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Shell() : ShadowIroncladCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SlatePower>(1m),
        new PowerVar<RetaliationPower>(3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SlatePower>(),
        HoverTipFactory.FromPower<RetaliationPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SlatePower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars.Power<SlatePower>().BaseValue,
            Owner.Creature, this);
        await PowerCmd.Apply<RetaliationPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars.Power<RetaliationPower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Power<RetaliationPower>().UpgradeValueBy(2m);
}
