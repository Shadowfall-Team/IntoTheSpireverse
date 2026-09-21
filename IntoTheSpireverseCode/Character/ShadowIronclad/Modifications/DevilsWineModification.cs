using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Potions;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Modifications;

/// <summary>
/// The only Modification applied by a potion rather than a card, so both loc lookups are pointed
/// at the potions table. Modification's own defaults read from "cards", which is right for every
/// other one.
///
/// Carries only the HP loss. Devil's Wine's Replay is written straight onto the card's
/// BaseReplayCount, because that is what the engine reads when it renders the card's own
/// "Replay N." line - a Replay granted through the play-count hook would work but would never
/// appear on the card.
/// </summary>
public sealed class DevilsWineModification : Modification
{
    protected override ModelId SourceCardId => ModelDb.Potion<DevilsWine>().Id;

    protected override LocString TitleLoc => new("potions", $"{SourceCardId.Entry}.title");

    public override LocString GetLoc(string subKey = CardTextSubKey)
    {
        var loc = new LocString("potions", $"{SourceCardId.Entry}.{subKey}");
        loc.Add("Amount", (decimal)Amount);
        DynamicVars.AddTo(loc);
        return loc;
    }

    /// <summary>
    /// Fires once per resolution, so a card carrying the Replay pays the HP for each of its plays.
    /// </summary>
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != Owner) return;

        await CreatureCmdCompatibility.Damage(choiceContext, Owner.Owner.Creature, (decimal)Amount,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, Owner, cardPlay);
    }
}
