using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Jettison() : ShadowRegentCard(2,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,
    };


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var cargoedCards = (await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(CargoSelectorPrefs.ToCargoSelectionPrompt, 0, 999999),
            null,
            this)).ToList();
        
        await CardPileCmd.Add(cargoedCards, CargoCardPile.CargoPileType);
        
        if (cargoedCards.Count != 0)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(cargoedCards.Count)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_starry_impact", null, "slash_attack.mp3")
                .Execute(choiceContext);
        }
    }
    
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
