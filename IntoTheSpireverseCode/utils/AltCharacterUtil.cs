using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;

namespace IntoTheSpireverse.IntoTheSpireverseCode.utils;

public static class AltCharacterUtil
{
    public static bool IsAvailableAltCharacter(CharacterModel c)
    {
        return c is IAltCharacter
               // && (c is not IIntoTheSpireverseDebug)
               ;
    }

    /// <summary>
    /// The "other side of the mirror" for a character: the base character if <paramref name="character"/> is an alt,
    /// otherwise every available alt built on top of it.
    /// </summary>
    public static IEnumerable<CharacterModel> GetMirrorCharacters(CharacterModel character)
    {
        if (character is IAltCharacter alt) return [alt.BaseCharacterModel];

        return ModelDb.AllCharacters.Where(c =>
            IsAvailableAltCharacter(c) && c is IAltCharacter ac && ac.BaseCharacterModel == character);
    }

    public static bool HasMirrorCharacter(CharacterModel character) => GetMirrorCharacters(character).Any();

    /// <summary>
    /// Cards that must never be offered to <paramref name="character"/>, because they are a base game / alt character
    /// duplicate of a card the character already has. You always keep the version belonging to the character you are
    /// playing, so an alt bans the base game halves and a base character bans its alts' halves.
    /// Only matters once the two halves of the mirror get merged - see ParallelStone and MirrorMirror.
    /// </summary>
    public static HashSet<ModelId> GetBannedCardIds(CharacterModel character)
    {
        if (character is IAltCharacter alt)
            return alt.DuplicateCardPairs.Select(p => p.BaseGame.Id).ToHashSet();

        return GetMirrorCharacters(character)
            .OfType<IAltCharacter>()
            .SelectMany(a => a.DuplicateCardPairs)
            .Select(p => p.Own.Id)
            .ToHashSet();
    }

    /// <summary>
    /// The canonical starting relic of a character. Every character declares exactly one, but StartingRelics is a
    /// list, so this returns null rather than throwing if that ever stops being true.
    /// </summary>
    public static RelicModel? GetStartingRelic(CharacterModel character) => character.StartingRelics.FirstOrDefault();
}
