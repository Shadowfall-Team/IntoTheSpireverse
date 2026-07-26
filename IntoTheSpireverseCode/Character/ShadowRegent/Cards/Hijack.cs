using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class Hijack() : ShadowRegentCard(
    1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1),
        // Unpowered: a steal transfers exactly what the enemy loses, so Dexterity must not inflate it.
        new BlockVar(12, ValueProp.Unpowered),
        new PowerVar<ShardsPower>(3),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<ShardsPower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var target = cardPlay.Target;

        // Only positive Strength can be taken, and never more than the enemy has.
        var stolenStrength = Math.Min(
            DynamicVars[nameof(StrengthPower)].BaseValue,
            Math.Max(0, target.GetPowerAmount<StrengthPower>()));

        if (stolenStrength > 0)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(),
                target, -stolenStrength, Owner.Creature, this);
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(),
                Owner.Creature, stolenStrength, Owner.Creature, this);
        }

        var stolenBlock = Math.Min(DynamicVars.Block.BaseValue, target.Block);

        if (stolenBlock > 0)
        {
            await CreatureCmd.LoseBlock(choiceContext, target, stolenBlock, Owner.Creature);
            await CreatureCmd.GainBlock(Owner.Creature, stolenBlock, ValueProp.Unpowered, cardPlay);
        }

        await PowerCmd.Apply<ShardsPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars[nameof(ShardsPower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(StrengthPower)].UpgradeValueBy(1);
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars[nameof(ShardsPower)].UpgradeValueBy(1);
    }
}
