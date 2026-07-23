using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards.Colorless;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Relics;

public class SneckoBrand : ShadowSilentRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicModel? GetUpgradeReplacement()
    {
        return ModelDb.Relic<SoulBrand>();
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Flicker>(),
    ];

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber != 1)
            return;

        Flash();

        var cards = Enumerable.Range(0, DynamicVars.Cards.IntValue)
            .Select(_ => combatState.CreateCard<Flicker>(Owner))
            .ToArray();

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
    }
    
}