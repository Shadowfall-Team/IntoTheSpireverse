using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards.Colorless;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Enchantments;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;


public sealed class Centurion() : ShadowSilentCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips 
    {
        get
        {
            List<IHoverTip> items = [];
            var card = ModelDb.Card<Scale>().ToMutable();
            CardCmd.Enchant<Armored>(card, 1);
            if (IsUpgraded) {
                CardCmd.Upgrade(card);
            }
            items.Add(HoverTipFactory.FromCard(card));
            items.AddRange(HoverTipFactory.FromEnchantment<Armored>());
            return items;
        }
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var scales = Enumerable.Range(0, DynamicVars.Cards.IntValue)
            .Select(c =>
            {
                var card = CombatState.CreateCard<Scale>(Owner);
                CardCmd.Enchant<Armored>(card, 1);
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(card);
                }
                return card;
            }); 
        await CardPileCmd.AddGeneratedCardsToCombat(scales ?? [], PileType.Hand, Owner);
    }
}
