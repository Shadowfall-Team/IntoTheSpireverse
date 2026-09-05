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

/// <summary>
/// Declares and displays StrengthPower but applies FootholdTemporaryStrengthPower, matching how the
/// base game's Setup Strike declares StrengthPower and applies SetupStrikePower.
/// </summary>
[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Foothold() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new PowerVar<StrengthPower>(2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<FootholdTemporaryStrengthPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars.Power<StrengthPower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.Power<StrengthPower>().UpgradeValueBy(1m);
    }
}
