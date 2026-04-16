using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Deathpact() : ShadowIroncladCard(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    private const string Glory2Name = "Glory2";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(17m, ValueProp.Move),
        new ShadowfallKeywords.GloryVar(5m),
        new ShadowfallKeywords.GloryVar(10m, Glory2Name),
        new PowerVar<StrengthPower>(3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ShadowfallKeywords.GloryHoverTipStatic(),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (ShadowfallKeywords.IsGloryTriggered(this, DynamicVars[ShadowfallKeywords.GloryVar.defaultName].IntValue))
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        if (ShadowfallKeywords.IsGloryTriggered(this, DynamicVars[Glory2Name].IntValue))
        {
            await PowerCmd.Apply<StrengthPower>(
                Owner.Creature, DynamicVars.Strength.BaseValue,
                Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
        DynamicVars.Strength.UpgradeValueBy(1m);
    }
}