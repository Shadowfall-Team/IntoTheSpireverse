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
public sealed class Megalith() : ShadowIroncladCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StoneHealthPower>(7m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StoneHealthPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<StoneHealthPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars.Power<StoneHealthPower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Power<StoneHealthPower>().UpgradeValueBy(3m);
}
