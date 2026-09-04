using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

/// <summary>
/// Terra Firma's Transcendence upgrade (Archaic Tooth). A craton is the ancient, stable core of a
/// continent - the oldest rock there is, which is about as literal an Ancient upgrade of "solid
/// ground" as geology offers.
///
/// Slate is granted before the replayed card resolves, so an Attack off the Discard pile is covered
/// by the Slate this card just gave.
/// </summary>
[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Craton() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SlatePower>(2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SlatePower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<SlatePower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature, DynamicVars.Power<SlatePower>().BaseValue,
            Owner.Creature, this);

        // CardPilePosition.Top resolves to index 0 for every pile.
        var top = PileType.Discard.GetPile(Owner).Cards.FirstOrDefault();
        if (top == null) return;

        await CardCmd.AutoPlay(choiceContext, top, null);
    }

    protected override void OnUpgrade() => DynamicVars.Power<SlatePower>().UpgradeValueBy(2m);
}
