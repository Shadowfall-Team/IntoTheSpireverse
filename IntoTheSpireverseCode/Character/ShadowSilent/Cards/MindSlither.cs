using MegaCrit.Sts2.Core.Animation;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards.Colorless;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;


public sealed class MindSlither() : ShadowSilentCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Flicker>(),
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        
        foreach (Creature teammate in CombatState.GetTeammatesOf(Owner.Creature).Where((c => c.IsAlive && c.IsPlayer)))
        {
            if (teammate.Player != null)
            {
                var flickers = Enumerable.Range(0, DynamicVars.Cards.IntValue)
                    .Select(c =>
                    {
                        var card = CombatState.CreateCard<Flicker>(teammate.Player);
                        CardCmd.RemoveKeyword(card, CardKeyword.Retain);
                        CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
                        return card;
                    }); 
                await CardPileCmd.AddGeneratedCardsToCombat(flickers ?? [], PileType.Hand, teammate.Player);
                await Cmd.Wait(0.1f);
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
