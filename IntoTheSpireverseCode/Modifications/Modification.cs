using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Modifications;

/// <summary>
/// Base for every effect that uses the Modify keyword. Modifications are CardModifiers rather
/// than Enchantments because a card has only one Enchantment slot, and Modify needs to coexist
/// with whatever else is already on the card.
///
/// The keyword promises "a Modified card cannot be Modified again", which is enforced here
/// rather than per-card: any Modification blocks any other, so future Modify cards inherit the
/// rule by deriving from this.
/// </summary>
public abstract class Modification : CardModifier
{
    /// <summary>
    /// The top bar's settings button sprite, shared by every Modification - on the card's flag
    /// and on its hover tip.
    /// </summary>
    public const string IconPath =
        "res://images/atlases/ui_atlas.sprites/top_bar/top_bar_settings.tres";

    public static bool IsModified(CardModel card) =>
        Modifiers(card).Any(modifier => modifier is Modification);

    public static bool CanModify(CardModel card) => !IsModified(card);

    /// <summary>
    /// The card whose "cards" localization entry holds this modification's title and text.
    /// </summary>
    protected abstract ModelId SourceCardId { get; }

    /// <summary>
    /// Whether this modification also appends its text to the card's description.
    ///
    /// False for modifications that only scale a value the card already prints: the damage and
    /// block previews run the modification hooks, so the printed number already includes the
    /// bonus and a line restating it reads as a second, additional bonus. Enchantments follow
    /// the same rule - they only carry extra card text when they add an effect the number
    /// cannot express.
    ///
    /// The hover tip is unaffected - every modification describes itself there.
    /// </summary>
    protected virtual bool AppendsTextToCardDescription => true;

    /// <summary>
    /// Titled after the card that applied it, matching how an enchantment's tip is titled after
    /// the enchantment.
    /// </summary>
    protected virtual LocString TitleLoc => new("cards", $"{SourceCardId.Entry}.title");

    /// <summary>
    /// The line appended to the modified card, phrased as one of that card's own effects:
    /// "Gain 3 Block and 1 Shard."
    /// </summary>
    public const string CardTextSubKey = "modifierText";

    /// <summary>
    /// The hover tip's body, phrased as what the modification does to a card rather than as an
    /// effect of the card: "Card gains \"Gain 3 Block and 1 Shard\"." The tip explains the
    /// modification in a vacuum, so it reads the same wherever it is shown.
    /// </summary>
    public const string TipSubKey = "modifierTip";

    /// <summary>
    /// No need to keep the text separate for loc purposes. The modifier text lives directly with the card
    /// that applies it.
    /// </summary>
    public override LocString GetLoc(string subKey = CardTextSubKey)
    {
        var loc = new LocString("cards", $"{SourceCardId.Entry}.{subKey}");
        loc.Add("Amount", (decimal)Amount);
        DynamicVars.AddTo(loc);
        return loc;
    }

    /// <summary>
    /// BaseLib folds these into the host card's HoverTips, the same list enchantment tips go
    /// into. Hovering a Modified card therefore describes the modification itself, the way
    /// hovering an enchanted card describes that enchantment rather than enchanting in general.
    /// Override and call base to add further tips.
    /// </summary>
    public override void AddTips(List<IHoverTip> tips)
    {
        var description = GetLoc(TipSubKey);
        if (!description.Exists())
        {
            MainFile.Logger.Warn($"Missing loc {description.LocTable}/{description.LocEntryKey}");
            return;
        }

        tips.Add(new HoverTip(TitleLoc, description, ResourceLoader.Load<Texture2D>(IconPath)));
    }

    /// <summary>
    /// Appends the modification's text to the card it is attached to, the way an Enchantment's
    /// extra card text does.
    /// </summary>
    public override void ModifyDescriptionPost(Creature? target, ref string description)
    {
        if (!AppendsTextToCardDescription) return;

        var loc = GetLoc(CardTextSubKey);
        if (!loc.Exists())
        {
            // Don't fail silently: a missing string looks identical to "the modification does
            // nothing" from in-game.
            MainFile.Logger.Warn($"Missing loc {loc.LocTable}/{loc.LocEntryKey}");
            return;
        }

        description += "\n" + loc.GetFormattedText();
    }
}
