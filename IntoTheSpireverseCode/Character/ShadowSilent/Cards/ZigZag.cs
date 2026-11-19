using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class ZigZag() : ShadowSilentCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    private const string _deviousKey = "Devious";
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move), new DynamicVar(_deviousKey, 0)];
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

        if (state.Iteration is not 0 || state.RepeatCount <= 0) return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardCompatibility(this, cardPlay).TargetingAllOpponents(CombatState).WithHitCount(state.RepeatCount).WithHitFx(VfxCmd.slashPath).Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars[_deviousKey].UpgradeValueBy(1);
}