using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class WomboCombo() : ShadowIroncladCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, ValueProp.Move),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var xCost = ResolveEnergyXValue();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(xCost)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (CombatManager.Instance.IsOverOrEnding) return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var card = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            [.. PileType.Discard.GetPile(Owner).Cards.Where(c => c.EnergyCost.GetResolved() <= xCost || c.EnergyCost.CostsX)],
            Owner,
            prefs)).FirstOrDefault();

        if (card == null) return;

        await CardCmd.AutoPlay(choiceContext, card, null);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5);
}
