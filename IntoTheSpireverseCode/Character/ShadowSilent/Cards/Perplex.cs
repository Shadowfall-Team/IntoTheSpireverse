using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Modifications;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class Perplex() : ShadowSilentCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> cardToPerplex = await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(Owner), Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), CanModify);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        foreach (CardModel cardModel in cardToPerplex)
        {
            CardModifier.AddModifier<SlitherModification>(cardModel);
            
            var child = NCardEnchantVfx.Create(cardModel);
            if (child != null)
            {
                var instance = NRun.Instance;
                if (instance != null)
                    instance.GlobalUi.CardPreviewContainer.AddChildSafely( child);
            }
        }
    }
    
    public static bool CanModify(CardModel card)
    {
        return Modification.CanModify(card) && IntoTheSpireverseKeywords.CanMuddle(card);
    }
    
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
