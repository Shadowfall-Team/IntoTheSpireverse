using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.ShadowfallCode.Keywords;
using Shadowfall.ShadowfallCode.Powers.ShadowNecrobinder;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class Patience() : ShadowNecrobinderCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private const string _patienceKey = "Patience";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(_patienceKey, 1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Linger)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<PatiencePower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars[_patienceKey].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[_patienceKey].UpgradeValueBy(1m);
    }
}