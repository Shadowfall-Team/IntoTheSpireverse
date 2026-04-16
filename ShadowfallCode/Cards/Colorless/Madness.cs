using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public sealed class Madness() : CustomCardModel(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(1m),
    ];

    public static IEnumerable<Madness> Create(Player owner, int amount, CombatState combatState)
    {
        List<Madness> list = new List<Madness>();
        for (int i = 0; i < amount; i++)
            list.Add(combatState.CreateCard<Madness>(owner));
        return list;
    }

    public static async Task<IEnumerable<Madness>> CreateInHand(Player owner, int amount, CombatState combatState)
    {
        IEnumerable<Madness> cards = Create(owner, amount, combatState);
        await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>)cards, PileType.Hand, true);
        return cards;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, Owner.Creature,
            DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, this);

        var hand = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c != this)
            .ToList();

        if (hand.Count == 0) return;

        var card = Owner.RunState.Rng.CombatCardSelection.NextItem(hand);
        card.EnergyCost.SetThisCombat(0);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}