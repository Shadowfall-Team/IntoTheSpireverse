using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class DarkBargain() : ShadowIroncladCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private bool IsPlayableChoice(CardModel card) =>
        card != this
        && !card.Keywords.Contains(CardKeyword.Unplayable)
        && !card.EnergyCost.CostsX;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var chosen = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, IsPlayableChoice, this))
            .FirstOrDefault();
        if (chosen == null) return;

        // Read the cost before playing: the card leaves its pile and its cost can be altered
        // (Corruption, free-this-turn effects) once it is on its way to the Play pile.
        var hpLoss = chosen.EnergyCost.GetResolved() + (IsUpgraded ? 0 : 1);

        await CardCmd.AutoPlay(choiceContext, chosen, null);

        if (hpLoss <= 0) return;

        await CreatureCmdCompatibility.Damage(choiceContext, Owner.Creature, hpLoss,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
    }

    protected override void OnUpgrade() { }
}
