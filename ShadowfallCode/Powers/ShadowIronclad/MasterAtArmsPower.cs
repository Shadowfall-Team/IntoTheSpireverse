using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class MasterAtArmsPower : CustomPowerModel
{
    private const int _energyIncrement = 3;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => _energyIncrement - GetInternalData<Data>().energySpent % _energyIncrement;
    public override bool IsInstanced => true;

    protected override object InitInternalData() => new Data();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Shiv>()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(3)
    ];

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature != Owner || amount <= 0)
            return;

        var data = GetInternalData<Data>();
        data.energySpent += amount;
        int triggers = data.energySpent / _energyIncrement - data.triggerCount;

        if (triggers > 0)
        {
            Flash();
            var cards = new List<CardModel>();
            for (int i = 0; i < triggers; i++)
                cards.Add(CombatState.CreateCard<Shiv>(Owner.Player));

            await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, true);
            data.triggerCount += triggers;
        }

        InvokeDisplayAmountChanged();
    }

    private class Data
    {
        public int energySpent;
        public int triggerCount;
    }
}