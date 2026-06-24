using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowNecrobinder;

public sealed class BoneVoyage() : ShadowNecrobinderCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(1m),
        new CardsVar(2),
        new EnergyVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromCard<SoulStrike>(),
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Linger),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        IntoTheSpireverseKeywords.Linger
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), cardPlay.Target, DynamicVars.Weak.BaseValue, Owner.Creature, this);
        var soulStrikes = SoulStrike.Create(Owner, DynamicVars.Cards.IntValue, CombatState).ToList();
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(
            (IEnumerable<CardModel>)soulStrikes, PileType.Draw, Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Weak.UpgradeValueBy(1m);
    }

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        int triggers = LingerHelper.GetTriggerCount(this);
        for (int i = 0; i < triggers; i++)
        {
            await PowerCmd.Apply<EnergyNextTurnPower>(new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars.Energy.BaseValue, Owner.Creature,
                this);
            await LingerHelper.NotifyLingerTriggered(this, choiceContext);
        }
    }
}
