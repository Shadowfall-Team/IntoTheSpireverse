using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Cards.Colorless.Rocks;

[Pool(typeof(TokenCardPool))]
public sealed class RollingRock() : RockCardBase(1, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
{
    private const string IncreaseKey = "Increase";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar(IncreaseKey, 3m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_rock_shatter", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);
        BuffFromRockPlay(DynamicVars[IncreaseKey].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars[IncreaseKey].UpgradeValueBy(3m);
}