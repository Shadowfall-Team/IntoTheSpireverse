using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class ZigZag() : ShadowSilentCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    private const string _deviousKey = "Devious";
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar(_deviousKey, 0m),
    ];
    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Devious];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IsUpgraded ? HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.DeviousX) : HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Devious),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await IntoTheSpireverseKeywords.ExecuteDevious(choiceContext, Owner, this, DynamicVars[_deviousKey].IntValue, () =>
            DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCardCompatibility(this, cardPlay)
                .TargetingAllOpponents(CombatState)
                .WithHitFx(VfxCmd.slashPath)
                .Execute(choiceContext));
	}

    protected override void OnUpgrade()
    {
        DynamicVars[_deviousKey].UpgradeValueBy(1m);
    }
}
