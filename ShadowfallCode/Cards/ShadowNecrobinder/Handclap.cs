using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class Handclap() : ShadowNecrobinderCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string _curseThresholdKey = "CurseThreshold";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new RepeatVar(2),
        new DynamicVar(_curseThresholdKey, 3m),
    ];

    public override TargetType TargetType =>
        HasEnoughCurses ? TargetType.AllEnemies : TargetType.AnyEnemy;

    protected override bool ShouldGlowGoldInternal => HasEnoughCurses;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_blunt");

        if (HasEnoughCurses)
            await attack.TargetingAllOpponents(CombatState).Execute(choiceContext);
        else
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await attack.Targeting(cardPlay.Target).Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    private bool HasEnoughCurses =>
        IsMutable &&
        Owner.PlayerCombatState != null &&
        Owner.PlayerCombatState.AllCards.Count(c => c.Type == CardType.Curse) >= DynamicVars[_curseThresholdKey].IntValue;
}
