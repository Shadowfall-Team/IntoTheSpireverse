using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class IceBeam() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(1),
        new("StrengthLoss", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<WeakPower>(
            new ThrowingPlayerChoiceContext(), cardPlay.Target, DynamicVars.Weak.BaseValue,
            Owner.Creature, this);

        await PowerCmd.Apply<IceBeamPower>(
            new ThrowingPlayerChoiceContext(), cardPlay.Target,
            DynamicVars["StrengthLoss"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthLoss"].UpgradeValueBy(1);
    }
}