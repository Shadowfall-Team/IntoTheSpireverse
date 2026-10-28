using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class VitalSurge() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust,
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StoneHealthPower>(2m),
        new EnergyVar(2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StoneHealthPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<StoneHealthPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<StoneHealthPower>().BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature,
            DynamicVars.Energy.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Power<StoneHealthPower>().UpgradeValueBy(2m);
}
