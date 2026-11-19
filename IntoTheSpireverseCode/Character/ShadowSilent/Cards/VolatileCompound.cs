using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class VolatileCompound() : ShadowSilentCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const string _deviousKey = "Devious";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VulnerablePower>(1), new (_deviousKey, 0)];
    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Devious];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromKeyword(IsUpgraded ? IntoTheSpireverseKeywords.DeviousX : IntoTheSpireverseKeywords.Devious)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        var state = DeviousEffectLoopTracker.GetState(cardPlay);
        if (!state.Initialized)
        {
            state.RepeatCount = Math.Max(0, await IntoTheSpireverseKeywords.ModifyDeviousRepeatCount(choiceContext, Owner, this, DynamicVars[_deviousKey].IntValue));
            state.Initialized = true;
        }

        if (state.Iteration >= state.RepeatCount) return;

        foreach (var hittableEnemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, hittableEnemy, DynamicVars.Vulnerable.BaseValue, cardPlay.Card.Owner.Creature, this);
            if (!hittableEnemy.HasPower<PoisonPower>()) continue;
            var power = hittableEnemy.GetPower<PoisonPower>();
            if (power is null) return;
            await CreatureCmdCompatibility.Damage(choiceContext, power.Owner, power.Amount, ValueProp.Unblockable | ValueProp.Unpowered, this, cardPlay);
            if (power.Owner.IsAlive) await PowerCmd.Decrement(power);
            else await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
    }

    protected override void OnUpgrade() => DynamicVars[_deviousKey].UpgradeValueBy(1);
}