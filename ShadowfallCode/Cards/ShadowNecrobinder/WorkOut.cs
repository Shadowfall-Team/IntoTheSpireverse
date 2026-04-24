using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Keywords;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class WorkOut() : ShadowNecrobinderCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    private const string _vigorKey = "Vigor";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1),
        new PowerVar<VigorPower>(_vigorKey, 2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<VigorPower>(),
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Linger)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        ShadowfallKeywords.Linger
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.Strength.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Strength.UpgradeValueBy(1m);
        DynamicVars[_vigorKey].UpgradeValueBy(1m);
    }

    public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        int triggers = LingerHelper.GetTriggerCount(this);
        for (int i = 0; i < triggers; i++)
        {
            await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars[_vigorKey].BaseValue, Owner.Creature, this);
            await LingerHelper.NotifyLingerTriggered(this, choiceContext);
        }
    }
}