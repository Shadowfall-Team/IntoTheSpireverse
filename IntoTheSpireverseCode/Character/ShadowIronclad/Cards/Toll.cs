using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

/// <summary>
/// Replaces Bloodletting, keeping its shape: HP traded for tempo. Instead of buying Energy it
/// buys one specific card outright, paying that card's Cost in HP.
/// </summary>
[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Toll() : ShadowIroncladCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const string DiscountKey = "Discount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(DiscountKey, 0m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var chosen = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
            c => c != this
                 && !c.Keywords.Contains(CardKeyword.Unplayable)
                 && !c.EnergyCost.CostsX,
            this)).FirstOrDefault();

        if (chosen == null) return;

        // Read the cost before playing: the card leaves Hand and its cost can be altered
        // (Corruption, free-this-turn effects) once it is on its way to the Play pile.
        var cost = Math.Max(0, chosen.EnergyCost.GetResolved() - DynamicVars[DiscountKey].IntValue);

        await CardCmd.AutoPlay(choiceContext, chosen, null);

        if (cost <= 0) return;

        await CreatureCmdCompatibility.Damage(choiceContext, Owner.Creature, cost,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars[DiscountKey].UpgradeValueBy(1m);
}
