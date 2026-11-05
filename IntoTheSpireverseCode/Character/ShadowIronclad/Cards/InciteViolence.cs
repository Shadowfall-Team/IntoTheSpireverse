using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class InciteViolence() : ShadowIroncladCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string RecoilKey = "Recoil";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new RepeatVar(2),
        new DynamicVar(RecoilKey, 1m),
        new DynamicVar(RecoilHitsKey, 2m),
    ];

    private const string RecoilHitsKey = "RecoilHits";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx(VfxCmd.slashPath)
            .Execute(choiceContext);

        if (cardPlay.Target?.IsDead != false || cardPlay.Target.Monster == null)
            return;

        InciteViolencePatch.IsIncitedAttack = true;
        var recoilResult = await DamageCmd.Attack(DynamicVars[RecoilKey].BaseValue)
            .WithHitCount(DynamicVars[RecoilHitsKey].IntValue)
            .FromMonsterSingleTarget(cardPlay.Target.Monster)
            .Targeting(Owner.Creature)
            .WithNoAttackerAnim()
            .WithHitFx(VfxCmd.bluntPath)
            .Execute(choiceContext);
        InciteViolencePatch.IsIncitedAttack = false;

        // Multiple fully blocked hits must still only stun once.
        var anyFullyBlocked = recoilResult.Results.Any(resultList => resultList.Any(r => r.WasFullyBlocked));
        if (anyFullyBlocked && cardPlay.Target.Monster.Creature.Powers.OfType<ImbalancedPower>().Any())
        {
            await CreatureCmd.Stun(cardPlay.Target.Monster.Creature);
            if (cardPlay.Target.Monster is BowlbugRock bowlbug)
                bowlbug.IsOffBalance = false;
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
