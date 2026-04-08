using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.CardPiles;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class FutureProofing() : ShadowRegentCard(1, CardType.Attack, CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Cargo)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (CombatState == null) return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);

        IEnumerable<CardModel> card = CargoCardPile.CargoPileType.GetPile(Owner)
            .Cards.Where(c => c.IsUpgradable)
            .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection);
        foreach (CardModel cardModel in card)
        {
            CardCmd.Upgrade(cardModel);
            CardCmd.Preview(cardModel);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}