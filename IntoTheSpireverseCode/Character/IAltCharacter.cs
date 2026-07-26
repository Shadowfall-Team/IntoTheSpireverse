using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

public interface IAltCharacter
{
    public CharacterModel BaseCharacterModel { get; }

    /// <summary>
    /// Cards this character owns that are functionally identical to a base game card, paired with that card.
    /// Effects that merge the two halves of the mirror (Parallel Stone, Mirror Mirror) would otherwise offer both
    /// halves of a pair and double its odds, so one half is always banned: you keep the version belonging to whichever
    /// character you are actually playing. See AltCharacterUtil.GetBannedCardIds.
    /// Cards that are only *similar* don't belong here - rename them and let them be their own card.
    /// </summary>
    public IEnumerable<(CardModel Own, CardModel BaseGame)> DuplicateCardPairs => [];
}