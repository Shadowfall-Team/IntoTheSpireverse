using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class ThousandCutsPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    protected override object InitInternalData() => new Data();

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != this.Owner)
            return Task.CompletedTask;
        this.GetInternalData<Data>().AmountsForPlayedCards.Add(cardPlay.Card, this.Amount);
        return Task.CompletedTask;
    }


    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int amount;
        if (cardPlay.Card.Owner.Creature != Owner || !GetInternalData<Data>().AmountsForPlayedCards.Remove(cardPlay.Card, out amount) || amount <= 0)
            return;
        
        VfxCmd.PlayOnCreatureCenters(CombatState.HittableEnemies, VfxCmd.slashPath);
        SfxCmd.Play(TmpSfx.slashAttack);

        Flash();
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), CombatState.HittableEnemies, amount, ValueProp.Unpowered, Owner);
    }
    

    private class Data
    {
        public readonly Dictionary<CardModel, int> AmountsForPlayedCards = new Dictionary<CardModel, int>();
    }
}
