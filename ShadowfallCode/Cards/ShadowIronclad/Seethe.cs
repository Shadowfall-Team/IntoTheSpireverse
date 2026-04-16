using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Keywords;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Seethe() : ShadowIroncladCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string Glory2Name = "Glory2";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new ShadowfallKeywords.GloryVar(2m),
        new ShadowfallKeywords.GloryVar(4m, Glory2Name),
        new EnergyVar(2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ShadowfallKeywords.GloryHoverTipStatic(),
        EnergyHoverTip,
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (ShadowfallKeywords.IsGloryTriggered(this, DynamicVars[ShadowfallKeywords.GloryVar.defaultName].IntValue))
        {
            var power = await PowerCmd.Apply<SeethingPower>(
                Owner.Creature, 1m, Owner.Creature, this);
            power.SetSelectedCard(this);
        }

        if (ShadowfallKeywords.IsGloryTriggered(this, DynamicVars[Glory2Name].IntValue))
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}