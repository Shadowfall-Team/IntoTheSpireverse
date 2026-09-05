using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ArtRoller;

/// <summary>
/// Works out which alt character a card is being shown "as", so base game cards reprinted into an
/// alt character's pool can carry their own art roll without affecting the original character.
///
/// A reprinted card is the *same* CardModel type as the base game's, so its id is shared. Scoping
/// the art roll by character keeps one id usable for both: Havoc drafted by the Ironclad renders as
/// the base game intends, while Havoc drafted by the Tectonic can be recoloured to match.
/// </summary>
public static class AltArtContext
{
    /// <summary>
    /// Separates card id from character id in a scoped art roll key, e.g.
    /// <c>CARD.HAVOC@INTOTHESPIREVERSE-SHADOW_IRONCLAD</c>. Chosen because it cannot occur in a
    /// ModelId, so an unscoped key can never be mistaken for a scoped one.
    /// </summary>
    public const char ScopeSeparator = '@';

    public static string ScopedKey(string cardId, CharacterModel character) =>
        $"{cardId}{ScopeSeparator}{character.Id.Entry}";

    /// <summary>
    /// Only base game cards need scoping. A card defined by this mod already belongs to exactly one
    /// character, so its unscoped roll is unambiguous and scoping it would just add a second key to
    /// keep in sync.
    /// </summary>
    public static bool NeedsScoping(CardModel card) =>
        card.GetType().Assembly == typeof(CardModel).Assembly;

    /// <summary>
    /// The key the roller should read and write for this card in the current context: scoped while
    /// an alt character is showing a base game reprint, plain otherwise. Saving through this is what
    /// lets a reprint be rolled from inside the alt character's pool without disturbing the
    /// original.
    /// </summary>
    public static string KeyFor(CardModel card)
    {
        string cardId = card.Id.ToString();
        var character = For(card);
        return character != null && NeedsScoping(card)
            ? ScopedKey(cardId, character)
            : cardId;
    }

    /// <summary>
    /// The alt character this card should be rendered as, or null when it should use its plain art.
    ///
    /// In combat and anywhere else the card has a real owner, the owner's character decides. In the
    /// compendium the cards are canonical and ownerless, so the library's selected pool filter
    /// decides instead - see <see cref="Patches.CardLibraryCharacterContextPatch"/>.
    /// </summary>
    public static CharacterModel? For(CardModel? card)
    {
        if (card == null) return null;

        // Owner asserts mutability and throws CanonicalModelException on a canonical card, so the
        // check has to come first rather than relying on a null return.
        if (!card.IsCanonical)
        {
            var owned = card.Owner?.Character;
            if (owned is IAltCharacter) return owned;

            // Owned by a real, non-alt character: never apply alt art.
            if (owned != null) return null;
        }

        var viewed = Patches.CardLibraryCharacterContextPatch.ViewedCharacter;
        return viewed is IAltCharacter ? viewed : null;
    }
}
