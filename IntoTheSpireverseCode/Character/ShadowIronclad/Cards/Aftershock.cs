using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Aftershock() : ShadowIroncladCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string PlaysKey = "Plays";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(PlaysKey, 2m),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatManager.Instance.IsOverOrEnding) return;

        var xCost = ResolveEnergyXValue();
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var card = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            [.. PileType.Discard.GetPile(Owner).Cards.Where(c => c.EnergyCost.GetResolved() <= xCost || c.EnergyCost.CostsX)],
            Owner,
            prefs)).FirstOrDefault();

        if (card == null) return;

        for (var i = 0; i < DynamicVars[PlaysKey].IntValue; i++)
        {
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
    }

    protected override void OnUpgrade() => DynamicVars[PlaysKey].UpgradeValueBy(1m);
}
