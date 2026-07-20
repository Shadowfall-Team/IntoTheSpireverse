using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class ThousandCuts() : ShadowSilentCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ThousandCutsPower>(1m),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<ThousandCutsPower>(
            choiceContext, Owner.Creature,
            DynamicVars.Power<ThousandCutsPower>().BaseValue,
            Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Power<ThousandCutsPower>().UpgradeValueBy(1m);
    }
}