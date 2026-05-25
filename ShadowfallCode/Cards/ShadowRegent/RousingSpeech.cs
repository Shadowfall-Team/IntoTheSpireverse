using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Ammo;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class RousingSpeech() : ShadowRegentCard(0,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new("Power", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var power = await PowerCmd.Apply<RousingSpeechPower>(choiceContext, Owner.Creature,
            1M, Owner.Creature, this);
        if (power == null) return;
        power.DynamicVars.Strength.BaseValue = DynamicVars["Power"].BaseValue;
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}

public class RousingSpeechPower : CustomPowerModel, IAmmoFiredListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override PowerStackType StackType =>
        DynamicVars["StrengthApplied"].IntValue != 0 ? PowerStackType.Counter : PowerStackType.None;

    public override int DisplayAmount => DynamicVars["StrengthApplied"].IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1),
        new("StrengthApplied", 0),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return Task.CompletedTask;
        return ApplyStrength(choiceContext);
    }

    public async void OnAmmoFired(Player player, IReadOnlyList<Creature> targets)
    {
        if (player.Creature != Owner) return;
        await ApplyStrength(new ThrowingPlayerChoiceContext());
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -DynamicVars["StrengthApplied"].BaseValue, Owner, null, silent: true);
    }

    private async Task ApplyStrength(PlayerChoiceContext choiceContext)
    {
        Flash();
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, DynamicVars.Strength.IntValue, Owner, null, silent: true);
        DynamicVars["StrengthApplied"].BaseValue += DynamicVars.Strength.IntValue;
        InvokeDisplayAmountChanged();
    }
}
