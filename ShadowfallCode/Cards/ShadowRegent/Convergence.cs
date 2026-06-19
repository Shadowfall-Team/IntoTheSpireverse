using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class Convergence() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new PowerVar<GainShardsNextTurnPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ShardPower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Retain),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<RetainHandPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1, Owner.Creature, this);

        await PowerCmd.Apply<EnergyNextTurnPower>(
            new ThrowingPlayerChoiceContext(),Owner.Creature,
            DynamicVars.Energy.BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<GainShardsNextTurnPower>(
            new ThrowingPlayerChoiceContext(),Owner.Creature,
            DynamicVars[nameof(GainShardsNextTurnPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(GainShardsNextTurnPower)].UpgradeValueBy(1);
    }
}