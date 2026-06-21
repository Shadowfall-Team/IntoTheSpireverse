using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

public sealed class PillarOfMutation() : ShadowIroncladCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(3m, ValueProp.Unpowered),
        new PowerVar<VigorPower>(1m),
        new PowerVar<PillarOfMutationPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromPower<VigorPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        (await PowerCmd.Apply<PillarOfMutationPower>(choiceContext,
            Owner.Creature, DynamicVars.Power<PillarOfMutationPower>().BaseValue, Owner.Creature, this)
        )?.AddVars(DynamicVars.Block.BaseValue, DynamicVars.Power<VigorPower>().BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(1m);
}
