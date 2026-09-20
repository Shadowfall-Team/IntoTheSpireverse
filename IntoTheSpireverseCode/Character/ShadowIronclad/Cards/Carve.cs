using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

/// <summary>
/// The damage increase is permanent for the run, so it follows the same shape as The Scythe and
/// The Law: the running total is a SavedProperty, and every play buffs the DeckVersion as well as
/// the combat copy so the deck entry carries the growth out of combat.
/// </summary>
[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Carve() : ShadowIroncladCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string IncreaseKey = "Increase";
    private const int BaseDamage = 6;
    private const int UpgradedBaseDamage = 8;

    private int _currentDamage = BaseDamage;
    private int _increasedDamage;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    [SavedProperty]
    public int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            AssertMutable();
            _currentDamage = value;
            DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get => _increasedDamage;
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentDamage, ValueProp.Move),
        new IntVar(IncreaseKey, 3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Indirectly),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx(VfxCmd.slashPath)
            .Execute(choiceContext);

        if (!IntoTheSpireverseKeywords.WasPlayedIndirectly(cardPlay)) return;

        var increase = DynamicVars[IncreaseKey].IntValue;
        BuffFromPlay(increase);
        (DeckVersion as Carve)?.BuffFromPlay(increase);
    }

    /// <summary>
    /// The upgrade has to go through UpdateDamage rather than DynamicVars.Damage.UpgradeValueBy,
    /// because that writes straight into BaseValue and the CurrentDamage setter writes the same
    /// field: whichever ran last would erase the other. Folding the upgrade into the same
    /// calculation keeps CurrentDamage the single source. CurrentUpgradeLevel is incremented before
    /// OnUpgrade runs, so IsUpgraded is already true here.
    /// </summary>
    protected override void OnUpgrade()
    {
        DynamicVars[IncreaseKey].UpgradeValueBy(1m);
        UpdateDamage();
    }

    /// <summary>
    /// CanonicalVars rebuilds Damage from the constants, so the accumulated growth has to be
    /// reapplied after a downgrade or it would be silently lost.
    /// </summary>
    protected override void AfterDowngraded() => UpdateDamage();

    private void BuffFromPlay(int extraDamage)
    {
        IncreasedDamage += extraDamage;
        UpdateDamage();
    }

    private void UpdateDamage() =>
        CurrentDamage = (IsUpgraded ? UpgradedBaseDamage : BaseDamage) + IncreasedDamage;
}
