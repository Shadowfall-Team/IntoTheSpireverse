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
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Bonebreaker() : ShadowIroncladCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const string Glory2Name = "Glory2";
    private const string StrengthLossKey = "StrengthLoss";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15m, ValueProp.Move),
        new ShadowfallKeywords.GloryVar(2m),
        new ShadowfallKeywords.GloryVar(4m, Glory2Name),
        new DynamicVar(StrengthLossKey, 10m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ShadowfallKeywords.GloryHoverTipStatic(),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        async Task Hit() => await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        async Task WeakenTarget() => await PowerCmd.Apply<BonebreakerPower>(
            cardPlay.Target, DynamicVars[StrengthLossKey].BaseValue,
            Owner.Creature, this);

        await Hit();

        if (ShadowfallKeywords.IsGloryTriggered(this, DynamicVars[ShadowfallKeywords.GloryVar.defaultName].IntValue))
        {
            await WeakenTarget();

            if (ShadowfallKeywords.IsGloryTriggered(this, DynamicVars[Glory2Name].IntValue))
            {
                await Hit();
                await WeakenTarget();
            }
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5m);
}