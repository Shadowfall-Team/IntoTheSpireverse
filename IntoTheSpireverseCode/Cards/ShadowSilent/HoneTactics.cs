using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

public sealed class HoneTactics() : ShadowSilentCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EnergyHoverTip,
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = (await CardSelectCmd.FromHand(
            choiceContext, Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1),
            null, this)).FirstOrDefault();

        if (card == null)
            return;

        int cost;
        if (card.EnergyCost.CostsX && Owner.PlayerCombatState != null)
            cost = Owner.PlayerCombatState.Energy;
        else
            cost = card.EnergyCost.GetWithModifiers(CostModifiers.All);

        await CardCmd.Discard(choiceContext, card);

        if (cost > 0)
            await PowerCmd.Apply<EnergyNextTurnPower>(
                choiceContext, Owner.Creature,
                cost,
                Owner.Creature, this);
    }

    protected override void OnUpgrade() =>
        EnergyCost.UpgradeBy(-1);
}
