using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class ShieldDrones() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.Self)
{
    private const string BlockNextTurnKey = nameof(BlockNextTurnPower);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8, ValueProp.Move),
        new BlockVar(BlockNextTurnKey, 4, ValueProp.Move)
    ];

    protected override bool ShouldGlowGoldInternal => HasColorlessInHand;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (CombatState == null) return;

        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        if (!HasColorlessInHand) return;

        var blockVar = (BlockVar)DynamicVars[BlockNextTurnKey];
        var blockNextTurnAmount = Hook.ModifyBlock(CombatState, Owner.Creature, blockVar.BaseValue, blockVar.Props,
            this, cardPlay, out _);
        await PowerCmd.Apply<BlockNextTurnPower>(
            choiceContext,
            Owner.Creature,
            blockNextTurnAmount,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars[BlockNextTurnKey].UpgradeValueBy(1);
    }

    private bool HasColorlessInHand =>
        PileType.Hand.GetPile(Owner).Cards.Any(c => c.Pool.IsColorless);
}