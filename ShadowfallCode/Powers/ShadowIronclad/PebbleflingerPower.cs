using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Shadowfall.ShadowfallCode.Cards.Colorless.Rocks;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class PebbleflingerPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;
    public override int DisplayAmount => 3 - GetInternalData<Data>().skillsPlayed % 3;

    protected override object InitInternalData() => new Data();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<SmallRock>(false),
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || cardPlay.Card.Type != CardType.Skill) return;
        var data = GetInternalData<Data>();
        data.skillsPlayed++;
        if (data.skillsPlayed % 3 == 0)
        {
            Flash();
            var rock = Owner.CombatState.CreateCard<SmallRock>(Owner.Player);
            await CardPileCmd.AddGeneratedCardsToCombat([rock], PileType.Hand, Owner.Player);
        }
        InvokeDisplayAmountChanged();
    }

    private class Data
    {
        public int skillsPlayed;
    }
}