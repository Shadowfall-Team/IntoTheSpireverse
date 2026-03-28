using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class TheEndIsComing() : ShadowNecrobinderCard(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    private int _timesPlayed;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(50m, ValueProp.Move),
        new DynamicVar("PlayCount", 0m),
    ];
    
    protected override bool ShouldGlowGoldInternal => _timesPlayed == 2;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _timesPlayed++;
        DynamicVars["PlayCount"].BaseValue = _timesPlayed;

        if (_timesPlayed >= 3)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(CombatState)
                .WithHitFx("vfx/vfx_giant_horizontal_slash")
                .Execute(choiceContext);
            await CardCmd.Exhaust(choiceContext, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(20m);
    }
}