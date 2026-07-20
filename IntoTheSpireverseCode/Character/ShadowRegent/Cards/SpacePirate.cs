using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;

public class SpacePirate() : ShadowRegentCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var flashOfSteel = (CardModel)ModelDb.Card<FlashOfSteel>().MutableClone();
            flashOfSteel.AddKeyword(CardKeyword.Exhaust);
            var fisticuffs = (CardModel)ModelDb.Card<Fisticuffs>().MutableClone();
            fisticuffs.AddKeyword(CardKeyword.Exhaust);
            var theBomb = (CardModel)ModelDb.Card<TheBomb>().MutableClone();
            theBomb.AddKeyword(CardKeyword.Exhaust);
            return
            [
                HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo),
                HoverTipFactory.FromCard(flashOfSteel, IsUpgraded),
                HoverTipFactory.FromCard(fisticuffs, IsUpgraded),
                HoverTipFactory.FromCard(theBomb, IsUpgraded)
            ];
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (cardPlay.Target != null)
        {
            var cardModel = CardFactory.GetDistinctForCombat(Owner,
                [
                    ModelDb.Card<FlashOfSteel>(),
                    ModelDb.Card<Fisticuffs>(),
                    ModelDb.Card<TheBomb>()
                ],
                1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if (cardModel != null)
            {
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(cardModel);
                }

                cardModel.AddKeyword(CardKeyword.Exhaust);

                var cardPileAddResult = await CardPileCmd.AddGeneratedCardToCombat(cardModel, CargoCardPile.CargoPileType,
                    Owner);
                CardCmd.PreviewCardPileAdd(cardPileAddResult);

            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
    }
}