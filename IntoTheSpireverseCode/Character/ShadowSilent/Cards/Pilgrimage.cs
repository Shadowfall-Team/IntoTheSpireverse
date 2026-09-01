using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class Pilgrimage() : ShadowSilentCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const string _discardKey = "Discard";
    private const string _deviousKey = "Devious";
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new DynamicVar(_discardKey, 1m),
        new DynamicVar(_deviousKey, 0m),
    ];
    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Devious];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IsUpgraded ? HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.DeviousX) : HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Devious),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        
        int maxDiscards = 1;
        foreach (var model in Owner.Creature.CombatState?.IterateHookListeners().ToList()!)
        {
            if (model is IntoTheSpireverseKeywords.IDeviousDiscardListener deviousListener)
                maxDiscards = deviousListener.ModifyDeviousDiscard(maxDiscards);
        }
        
        var cards = (await CardSelectCmd.FromHandForDiscard(
            choiceContext,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1, Math.Max(maxDiscards,1)),
            null,
            this));

        int repeats = DynamicVars[_deviousKey].IntValue;
        foreach (CardModel card in cards)
        {
            if (card.Owner.Creature.CombatState == null) return;
            
            repeats += Math.Max(0, card.EnergyCost.GetWithModifiers(CostModifiers.All));
            if (card.EnergyCost.CostsX && Owner.PlayerCombatState != null)
                repeats += Owner.PlayerCombatState.Energy;
            await CardCmd.Discard(choiceContext, card);
            
            foreach (var model in card.Owner.Creature.CombatState.IterateHookListeners().ToList())
            {
                if (model is IntoTheSpireverseKeywords.IModifyDeviousListener deviousListener)
                    repeats = deviousListener.ModifyDeviousValue(card, repeats);
            }
        }

        if (repeats > 0)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue*repeats, Owner);
            await CardCmd.Discard(choiceContext, await CardSelectCmd.FromHandForDiscard(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, DynamicVars[_discardKey].IntValue*repeats),  null, this));
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[_deviousKey].UpgradeValueBy(1m);
    }
}
