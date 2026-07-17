using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowNecrobinder;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowNecrobinder;

public sealed class Voodoo() : ShadowNecrobinderCard(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const string _strengthLossKey = "StrengthLoss";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(_strengthLossKey, 4m),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        IntoTheSpireverseKeywords.Pickup
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromCard<Normality>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, this);
        await PowerCmd.Apply<VoodooPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, DynamicVars[_strengthLossKey].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[_strengthLossKey].UpgradeValueBy(2m);
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPile, AbstractModel? source)
    {
        if (card == this && card.Pile?.Type == PileType.Deck)
        {
            await CardPileCmd.AddCursesToDeck(Enumerable.Repeat(ModelDb.Card<Normality>(), 1), Owner);
        }
    }
}