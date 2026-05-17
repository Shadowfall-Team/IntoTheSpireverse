using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
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
        new CalculationBaseVar(8),
        new ExtraDamageVar(1),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(static (card, _) =>
                card.Owner.Creature.GetPowerAmount<VolleyDamageThisTurnPower>() +
                card.Owner.Creature.GetPowerAmount<VolleyDamagePower>()),
        new RepeatVar(0),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        // CardKeyword.Retain,
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        // HoverTipFactory.FromPower<StrengthPower>(),
    ];


    protected override async Task OnPlay(PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        var command = DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .WithAttackerAnim("Cast", Owner.Character.AttackAnimDelay)
            .WithAttackerFx(null, "event:/sfx/characters/regent/regent_sovereign_blade", null);

        if (Owner.HasPower<BigGunsPower>())
        {
            command.TargetingAllOpponents(CombatState);
        }
        else
        {
            command.TargetingRandomOpponents(CombatState);
        }

        await command.Execute(choiceContext);

        await Cleanup();
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
        DynamicVars.CalculationBase.BaseValue += amount;
        CurrentDamage = DynamicVars.CalculationBase.BaseValue;
    }
}