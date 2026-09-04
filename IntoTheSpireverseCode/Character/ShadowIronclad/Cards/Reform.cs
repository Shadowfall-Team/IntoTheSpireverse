using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

/// <summary>
/// Replaces Rock Collection. Now that every Rock Exhausts, the Exhaust pile is where the
/// character's spent Rocks accumulate, so this reads as Rock recursion rather than as a generic
/// Exhaust payoff.
/// </summary>
[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Reform() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string AttacksKey = "Attacks";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(AttacksKey, 3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // Snapshot first: each play mutates the Exhaust pile as cards leave and re-enter it.
        var attacks = PileType.Exhaust.GetPile(Owner).Cards
            .Where(c => c.Type == CardType.Attack && c != this)
            .TakeRandom(DynamicVars[AttacksKey].IntValue, Owner.RunState.Rng.CombatCardSelection)
            .ToList();

        foreach (var attack in attacks)
            await CardCmd.AutoPlay(choiceContext, attack, null);
    }

    protected override void OnUpgrade() => DynamicVars[AttacksKey].UpgradeValueBy(1m);
}
