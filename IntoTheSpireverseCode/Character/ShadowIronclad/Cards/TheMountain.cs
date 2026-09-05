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

/// <summary>
/// Slate is granted before the replayed card resolves, so an Attack off the Discard pile is covered
/// by the Slate this card just gave.
/// </summary>
[Pool(typeof(ShadowIroncladCardPool))]
public sealed class TheMountain() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SlatePower>(3m),
    ];

    /// <summary>
    /// "The top card of your Discard Pile" is not something the player can see, so the card it is
    /// about to play is previewed on hover, the way Rock-making cards preview the Rock they create.
    /// CardModel.HoverTips rebuilds this list on every access rather than caching it, so the
    /// preview tracks the pile as it changes.
    /// </summary>
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

    /// <summary>
    /// Null whenever there is nothing meaningful to preview. Every check here is load-bearing:
    /// hover tips are also built for canonical cards in the compendium and card library, where
    /// there is no player and no combat, and CardModel.Owner throws CanonicalModelException rather
    /// than returning null on such an instance.
    /// </summary>
    private CardModel? TryGetTopOfDiscard()
    {
        if (IsCanonical) return null;
        if (CombatState == null) return null;

        var owner = Owner;
        if (owner == null) return null;

        // CardPilePosition.Top resolves to index 0 for every pile.
        return PileType.Discard.GetPile(owner)?.Cards.FirstOrDefault();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<SlatePower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars.Power<SlatePower>().BaseValue,
            Owner.Creature, this);

        var top = TryGetTopOfDiscard();
        if (top == null) return;

        await CardCmd.AutoPlay(choiceContext, top, null);
    }

    protected override void OnUpgrade() => DynamicVars.Power<SlatePower>().UpgradeValueBy(2m);
}
