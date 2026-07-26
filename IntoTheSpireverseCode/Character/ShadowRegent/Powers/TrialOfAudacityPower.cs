using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;

public sealed class TrialOfAudacityPower : ShadowPowerModel
{
    public const int RequiredCards = 5;

    public const string RequiredKey = "Required";

    // Live count of unique Colorless cards, refreshed on every recount so the
    // tooltip can show "You have {Current} unique Colorless cards."
    public const string CurrentKey = "Current";

    public override PowerType Type => PowerType.Buff;

    // Counter so Amount displays the damage; extra copies stack it additively.
    public override PowerStackType StackType => PowerStackType.Counter;

    // Required exists so the loc string can read the threshold instead of hardcoding it.
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(RequiredKey, RequiredCards),
        new DynamicVar(CurrentKey, 0)
    ];

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await TryTrigger();
    }

    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        // Gated on Colorless so the recount only runs when it could actually change.
        if (card.Owner != Owner.Player || !card.Pool.IsColorless) return;
        await TryTrigger();
    }

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card.Owner != Owner.Player || !card.Pool.IsColorless) return;
        await TryTrigger();
    }

    private async Task TryTrigger()
    {
        var player = Owner.Player;
        if (player == null || CombatState == null) return;

        var count = CountUniqueColorlessCards(player);
        DynamicVars[CurrentKey].BaseValue = count;

        if (count < DynamicVars[RequiredKey].IntValue) return;

        Flash();
        await Cmd.Wait(0.5f);
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
            CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner);

        await PowerCmd.Remove(this);
    }

    private static int CountUniqueColorlessCards(Player player)
    {
        var combat = player.PlayerCombatState;
        if (combat == null) return 0;

        return combat.AllCards
            .Where(card => card.Pool.IsColorless)
            .Select(card => card.Id)
            .Distinct()
            .Count();
    }
}
