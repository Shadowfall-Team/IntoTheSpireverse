using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class InciteViolence() : ShadowIroncladCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string RecoilKey = "Recoil";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new RepeatVar(2),
        new DynamicVar(RecoilKey, 2m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        InciteViolencePatch.IsIncitedAttack = true;
        await DamageCmd.Attack(DynamicVars[RecoilKey].BaseValue)
            .FromMonster(cardPlay.Target.Monster)
            .WithNoAttackerAnim() // Remove this to make enemies visually attack with the retaliatory damage, though it causes visual quirks with Tough Egg, etc
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        InciteViolencePatch.IsIncitedAttack = false;
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}