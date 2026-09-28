using MegaCrit.Sts2.Core.Animation;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards.Rocks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Quarry() : ShadowIroncladCard(-1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        IsUpgraded
            ? [HoverTipFactory.FromCard<MediumRock>(), HoverTipFactory.FromCard<SmallRock>()]
            : [HoverTipFactory.FromCard<MediumRock>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        var rocks = new List<CardModel>();
        for (var i = 0; i < ResolveEnergyXValue(); i++)
            rocks.Add(CombatState.CreateCard<MediumRock>(Owner));

        // The upgrade adds a body rather than another X, so a high-energy Quarry does not scale it.
        if (IsUpgraded) rocks.Add(CombatState.CreateCard<SmallRock>(Owner));

        if (rocks.Count == 0) return;
        await CardPileCmd.AddGeneratedCardsToCombat(rocks, PileType.Hand, Owner);
    }
}
