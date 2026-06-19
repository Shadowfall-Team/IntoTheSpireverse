using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
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
        new DamageVar(6m, ValueProp.Move),
        new RepeatVar(2),
        new DynamicVar(RecoilKey, 1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (attackCommand.Results.SelectMany( r =>  r).Any( r => r.WasTargetKilled))
            return;

        InciteViolencePatch.IsIncitedAttack = true;
        var recoilResult = await DamageCmd.Attack(DynamicVars[RecoilKey].BaseValue)
            .FromMonsterSingleTarget(cardPlay.Target.Monster)
            .Targeting(Owner.Creature)
            .WithNoAttackerAnim()
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        InciteViolencePatch.IsIncitedAttack = false;

        foreach (var resultList in recoilResult.Results)
        {
            foreach (var result in resultList)
            {
                if (!result.WasFullyBlocked) continue;
                var imbalanced = cardPlay.Target.Monster.Creature.Powers
                    .OfType<ImbalancedPower>()
                    .FirstOrDefault();
                if (imbalanced == null) continue;
                await CreatureCmd.Stun(cardPlay.Target.Monster.Creature);
                if (cardPlay.Target.Monster is BowlbugRock bowlbug)
                    bowlbug.IsOffBalance = false;
            }
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
