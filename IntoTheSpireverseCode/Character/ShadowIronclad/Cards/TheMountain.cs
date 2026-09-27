using MegaCrit.Sts2.Core.Animation;
using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class TheMountain() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SlatePower>(2m),
    ];

    // Previews the card it will play. Rebuilt on every access, so it tracks the pile.
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var tips = new List<IHoverTip> { HoverTipFactory.FromPower<SlatePower>() };

            var top = TryGetTopOfDiscard();
            if (top != null) tips.Add(HoverTipFactory.FromCard(top));

            return tips;
        }
    }

    // IsCanonical first: hover tips are built for compendium cards too, where Owner throws.
    private CardModel? TryGetTopOfDiscard()
    {
        if (IsCanonical || CombatState == null) return null;

        // Discards are appended, so the most recent is last (unlike the Draw Pile's top at index 0).
        return PileType.Discard.GetPile(Owner)?.Cards.LastOrDefault();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        // Slate first, so a replayed Attack is covered by it.
        await PowerCmd.Apply<SlatePower>(
            choiceContext,
            Owner.Creature, DynamicVars.Power<SlatePower>().BaseValue,
            Owner.Creature, this);

        var top = TryGetTopOfDiscard();
        if (top == null) return;

        await CardCmd.AutoPlay(choiceContext, top, null);
    }

    protected override void OnUpgrade() => DynamicVars.Power<SlatePower>().UpgradeValueBy(2m);
}
