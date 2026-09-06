using MegaCrit.Sts2.Core.Animation;
using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class ClaySoldier() : ShadowIroncladCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1m),
        new BlockVar(3m, ValueProp.Unpowered),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<ClaySoldierPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars.Power<StrengthPower>().BaseValue,
            Owner.Creature, this);

        (await PowerCmd.Apply<ClaySoldierPower>(choiceContext,
            Owner.Creature, 1m, Owner.Creature, this)
        )?.AddVars(DynamicVars.Block.BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Power<StrengthPower>().UpgradeValueBy(1m);
}
