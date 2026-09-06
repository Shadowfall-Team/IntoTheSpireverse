using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards.Rocks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public sealed class PebbleflingerPower : ShadowPowerModel
{
    private const int EnergyPerRock = 3;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override int DisplayAmount => EnergyPerRock - GetInternalData<Data>().energySpent % EnergyPerRock;

    protected override object InitInternalData() => new Data();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<SmallRock>(false),
    ];

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (Owner.CombatState == null || card.Owner != Owner.Player || amount <= 0) return;

        var data = GetInternalData<Data>();
        var before = data.energySpent;
        data.energySpent += amount;

        // A single expensive card can cross the threshold more than once.
        var rocks = data.energySpent / EnergyPerRock - before / EnergyPerRock;
        if (rocks > 0)
        {
            Flash();
            var created = new List<CardModel>();
            for (var i = 0; i < rocks; i++)
                created.Add(Owner.CombatState.CreateCard<SmallRock>(Owner.Player));
            await CardPileCmd.AddGeneratedCardsToCombat(created, PileType.Hand, Owner.Player);
        }

        InvokeDisplayAmountChanged();
    }

    private class Data
    {
        public int energySpent;
    }
}
