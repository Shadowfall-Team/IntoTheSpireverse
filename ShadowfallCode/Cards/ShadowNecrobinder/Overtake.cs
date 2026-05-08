using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Keywords;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class Overtake() : ShadowNecrobinderCard(0, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(2),
        new PowerVar<VulnerablePower>(2),
        new CardsVar(2),
        new EnergyVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Linger)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        ShadowfallKeywords.Linger
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, DynamicVars.Weak.BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Weak.UpgradeValueBy(1m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        int triggers = LingerHelper.GetTriggerCount(this);
        for (int i = 0; i < triggers; i++)
        {
            await PowerCmd.Apply<DrawCardsNextTurnPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.Cards.BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<EnergyNextTurnPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.Energy.BaseValue, Owner.Creature, this);
            await LingerHelper.NotifyLingerTriggered(this, choiceContext);
        }
    }
}
