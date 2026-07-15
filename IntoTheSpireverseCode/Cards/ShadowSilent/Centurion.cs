using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Enchantments;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
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
            List<IHoverTip> items = new List<IHoverTip>();
            items.Add(HoverTipFactory.FromCard<Scale>(IsUpgraded));
            items.AddRange(HoverTipFactory.FromEnchantment<Armored>());
            return items;
        }
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var scales = Enumerable.Range(0, DynamicVars.Cards.IntValue)
            .Select(_ => CombatState.CreateCard<Scale>(Owner))
            .ToArray();
        foreach (Scale card in scales)
        {
            CardCmd.Enchant<Armored>(card, 1M);
            if (IsUpgraded)
            {
                CardCmd.Upgrade(card);
            }
        }
        await CardPileCmd.AddGeneratedCardsToCombat(scales, PileType.Hand, Owner);
    }
}