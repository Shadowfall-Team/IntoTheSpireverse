using MegaCrit.Sts2.Core.Animation;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Potions;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

/// <summary>
/// Worded as a Transform so it needs a card to consume. "Exhaust a card, procure a potion" would
/// still hand over the potion from an otherwise empty Hand.
/// </summary>
[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Alkalize() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPotion<PotionShapedRock>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
        var original = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this))
            .FirstOrDefault();
        if (original == null) return;

        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        await CardCmdCompatibility.Exhaust(choiceContext, original);
        await PotionCmd.TryToProcure<PotionShapedRock>(Owner);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
