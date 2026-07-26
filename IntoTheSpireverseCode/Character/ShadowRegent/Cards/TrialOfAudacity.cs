using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class TrialOfAudacity() : ShadowRegentCard(1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    private const string TrialDamageKey = "TrialDamage";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(TrialDamageKey, 20),
        // Mirrors the power's threshold so both loc strings read one source of truth.
        new DynamicVar(TrialOfAudacityPower.RequiredKey, TrialOfAudacityPower.RequiredCards)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await PowerCmd.Apply<TrialOfAudacityPower>(choiceContext,
            Owner.Creature,
            DynamicVars[TrialDamageKey].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[TrialDamageKey].UpgradeValueBy(10);
    }
}
