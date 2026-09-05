using MegaCrit.Sts2.Core.Animation;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards.Colorless;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;


public sealed class Shimmer() : ShadowSilentCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Flicker>(true)
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        var flickers = Enumerable.Range(0, DynamicVars.Cards.IntValue)
            .Select(c =>
            {
                var card = CombatState.CreateCard<Flicker>(Owner);
                CardCmd.Upgrade(card);
                return card;
            }); 
        await CardPileCmd.AddGeneratedCardsToCombat(flickers ?? [], PileType.Hand, Owner);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
