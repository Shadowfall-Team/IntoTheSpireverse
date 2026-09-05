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
public sealed class SculptReality() : ShadowIroncladCard(2, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SculptRealityPower>(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SculptRealityPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars.Power<SculptRealityPower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
