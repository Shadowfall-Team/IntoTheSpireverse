using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public sealed class AmbushPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData() => new Data();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Weight>(false),
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(0),
    ];

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (card.Owner.Creature != Owner)
            return false;
        if (card.Pile?.Type is not (PileType.Hand or PileType.Play))
            return false;

        GetInternalData<Data>().originalCosts[card] = (int)originalCost;
        modifiedCost = 0m;
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        Flash();

        var data = GetInternalData<Data>();
        int originalCost = data.originalCosts.TryGetValue(cardPlay.Card, out var stored)
            ? stored
            : 0;

        if (cardPlay.Card.EnergyCost.CostsX)
            originalCost = Owner.Player.PlayerCombatState?.Energy ?? 0;

        if (originalCost > 0)
        {
            var weights = Enumerable.Range(0, originalCost)
                .Select(_ => CombatState.CreateCard<Weight>(Owner.Player))
                .ToArray();
            await CardPileCmd.AddGeneratedCardsToCombat(
                weights, PileType.Hand, Owner.Player);
        }

        await PowerCmd.Decrement(this);
    }

    private class Data
    {
        public readonly Dictionary<CardModel, int> originalCosts = new();
    }
}