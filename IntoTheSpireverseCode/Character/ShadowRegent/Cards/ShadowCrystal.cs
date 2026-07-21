using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class ShadowCrystal() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Crystalline>();

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        // minCount 0 so a hand with fewer eligible cards than Cards can't stall the selector.
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, minCount: 0,
            DynamicVars.Cards.IntValue);

        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
            CanEnchant, this);

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);

        foreach (var card in selected)
        {
            CardCmd.Enchant<Crystalline>(card, 1m);

            var vfx = NCardEnchantVfx.Create(card);
            if (vfx != null)
                NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(vfx);
        }
    }

    // One-slot system, so already-enchanted cards are not offered.
    public static bool CanEnchant(CardModel card) =>
        card.Enchantment == null && ModelDb.Enchantment<Crystalline>().CanEnchant(card);

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
