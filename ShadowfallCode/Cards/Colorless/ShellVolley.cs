using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public class AmmoVolley() : CustomCardModel(1,
    CardType.Attack,
    CardRarity.Token,
    TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, ValueProp.Move),
        new RepeatVar(0),
        
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        // HoverTipFactory.FromPower<StrengthPower>(),
    ];


    protected override async Task OnPlay(PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        /*
         await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .TargetingRandomOpponents(CombatState)
            .WithAttackerAnim("Cast", Owner.Character.AttackAnimDelay, null)
            .Execute(choiceContext);
            // .WithAttackerFx(null, "event:/sfx/characters/regent/regent_sovereign_blade", null)
            */

            for (int i = 0; i < DynamicVars.Repeat.IntValue; i++)
            {
                //TODO: move this to a calculated var?
                var volleyDamage = DynamicVars.Damage.BaseValue +
                                   Owner.Creature.GetPowerAmount<VolleyDamageThisTurnPower>() +
                                   Owner.Creature.GetPowerAmount<VolleyDamagePower>();

                // if (Owner.HasPower<StrengthVolleyPower>())
                // {
                //     volleyDamage += Owner.Creature.GetPowerAmount<StrengthPower>();
                // }
            
                var target = SelectTarget();

                if (target == null) return;
            
                //TODO: maybe play an animation here?
                // VfxCmd.PlayOnCreatureCenter(attackCommand.Attacker, attackCommand._attackerVfx);

                await CreatureCmd.Damage(choiceContext, target, volleyDamage,
                    ValueProp.Move, Owner.Creature);

                if (Owner.HasPower<CascadePower>())
                {
                    await PowerCmd.Apply<VolleyDamagePower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1, Owner.Creature, null);
                }

                if (Owner.HasPower<SiegePower>())
                {
                    await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, 1, Owner.Creature, null);
                }

                if (Owner.HasPower<DefensiveCannonadePower>())
                {
                    await CreatureCmd.GainBlock(Owner.Creature, Owner.Creature.GetPowerAmount<DefensiveCannonadePower>(), ValueProp.Move, null);
                }
            }
    }
    
    private Creature? SelectTarget()
    {
        var validTargets = CombatState.Enemies.Where(e => e.IsAlive).ToList();
        var preferredTargets = validTargets
            .Where(t => t.HasPower<TargetedThisTurnPower>()).ToList();

        var target = CombatState.RunState.Rng.CombatTargets.NextItem(
            preferredTargets.Count != 0 ? preferredTargets : validTargets);
        return target;
    }

    private async Task Cleanup()
    {
        await PowerCmd.Remove<VolleyDamageThisTurnPower>(Owner.Creature);
        await PowerCmd.Remove<DefensiveCannonadePower>(Owner.Creature);
        foreach (var target in
                 CombatState.Enemies.Where(e => e.HasPower<TargetedThisTurnPower>()))
        {
            await PowerCmd.Remove<TargetedThisTurnPower>(target);
        }

        // await PowerCmd.Remove(Owner.Creature);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public override TargetType TargetType
    {
        get
        {
            // if (!this.HasSeekingEdge)
            // {
            return TargetType.RandomEnemy;
            // }
            // return TargetType.AllEnemies;
        }
    }

    private decimal CurrentDamage
    {
        get { return field; }
        set
        {
            AssertMutable();
            field = value;
        }
    } = 10;

    private decimal CurrentRepeats
    {
        get { return field; }
        set
        {
            AssertMutable();
            field = value;
        }
    } = 1;

    public void SetRepeats(decimal amount)
    {
        DynamicVars.Repeat.BaseValue = amount;
        CurrentRepeats = DynamicVars.Repeat.BaseValue;
    }

    public void AddRepeats(decimal amount)
    {
        DynamicVars.Repeat.BaseValue += amount;
        CurrentRepeats = DynamicVars.Repeat.BaseValue;
    }

    public void AddDamage(decimal amount)
    {
        DynamicVars.Damage.BaseValue += amount;
        CurrentDamage = DynamicVars.Damage.BaseValue;
    }
}