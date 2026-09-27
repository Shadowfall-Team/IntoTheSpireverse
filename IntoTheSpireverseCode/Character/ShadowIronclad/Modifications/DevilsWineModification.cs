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
/// Applied by a potion, so loc reads from the potions table. Carries only the HP loss: the Replay
/// is set on BaseReplayCount, the only place the card's own "Replay N." line reads from.
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

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != Owner) return;

        await CreatureCmdCompatibility.Damage(choiceContext, Owner.Owner.Creature, (decimal)Amount,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, Owner, cardPlay);
    }
}
