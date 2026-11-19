using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class DivineIntervention() : ShadowSilentCard(3, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    private const string _deviousKey = "Devious";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DivineInterventionPower>(1), new (_deviousKey, 0)];
    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Devious];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(IsUpgraded ? IntoTheSpireverseKeywords.DeviousX : IntoTheSpireverseKeywords.Devious)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null) return;
        var state = DeviousEffectLoopTracker.GetState(cardPlay);
        if (!state.Initialized)
        {
            state.RepeatCount = Math.Max(0, await IntoTheSpireverseKeywords.ModifyDeviousRepeatCount(choiceContext, Owner, this, DynamicVars[_deviousKey].IntValue));
            state.Initialized = true;
        }

        if (state.Iteration >= state.RepeatCount) return;

        var list = CombatState.GetTeammatesOf(Owner.Creature).Where(c => c is { IsAlive: true, IsPlayer: true } && c.Player != Owner).ToList();
        if (list.Count is 0) return;
        var target = Owner.RunState.Rng.CombatTargets.NextItem(list)?.Player?.Creature;
        if (target is not null)
        {
            if (target.HasPower<DivineInterventionPower>()) await PowerCmd.Apply<DivineInterventionPower>(choiceContext, target, DynamicVars["DivineInterventionPower"].BaseValue, Owner.Creature, this);
            else await PowerCmd.Apply<DivineInterventionPower>(choiceContext, target, DynamicVars["DivineInterventionPower"].BaseValue + 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars[_deviousKey].UpgradeValueBy(1);
}