using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

public class Misdirection() : ShadowRegentCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9, ValueProp.Move),
        new CardsVar(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Cargo)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        var cargoPile = CargoCardPile.CargoPileType.GetPile(Owner);
        if (!cargoPile.IsEmpty)
        {
            var cardModels = cargoPile.Cards.Take((int)DynamicVars.Cards.BaseValue).ToList();
            foreach (var cardModel in cardModels)
            {
                await CardPileCmd.Add(cardModel, PileType.Hand);
                if (Owner.Creature.CombatState == null) continue;
                await Hook.AfterCardDrawn(Owner.Creature.CombatState, choiceContext, cardModel, true);
            }
        }
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}