using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Blitz() : ShadowIroncladCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const string HitsKey = "Hits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        // Single-target hits are fixed; the sweep is what the upgrade improves, so it uses the
        // Repeat var and carries the :diff() marker on the card.
        new DynamicVar(HitsKey, 2m),
        new RepeatVar(2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null || cardPlay.Target == null) return;

        await using var context = await AttackCommand.CreateContextAsync(CombatState, choiceContext, cardPlay);

        for (int i = 0; i < DynamicVars[HitsKey].IntValue; i++)
        {
            var hits = await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage, Owner.Creature);
            context.AddHit(hits);
        }

        for (int i = 0; i < DynamicVars.Repeat.IntValue; i++)
        {
            var hits = await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, DynamicVars.Damage, Owner.Creature);
            context.AddHit(hits);
        }

        // MAYBE: Check if we can still have the vfx for the async context hits?
        // await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
        //     .WithHitCount(DynamicVars[HitsKey].IntValue)
        //     .FromCardCompatibility(this, cardPlay)
        //     .Targeting(cardPlay.Target)
        //     .WithHitFx(VfxCmd.slashPath)
        //     .Execute(choiceContext);

        // await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
        //     .WithHitCount(DynamicVars.Repeat.IntValue)
        //     .FromCardCompatibility(this, cardPlay)
        //     .TargetingAllOpponents(CombatState)
        //     .WithHitFx(VfxCmd.slashPath)
        //     .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Repeat.UpgradeValueBy(1m);
}
