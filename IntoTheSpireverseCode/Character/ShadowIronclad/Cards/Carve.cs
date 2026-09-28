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
/// Growth lasts the run, as with The Scythe: it is a SavedProperty and is mirrored onto DeckVersion.
/// </summary>
[Pool(typeof(ShadowIroncladCardPool))]
public sealed class Carve() : ShadowIroncladCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string IncreaseKey = "Increase";
    private const int BaseDamage = 8;
    private const int UpgradedBaseDamage = 10;

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
        new IntVar(IncreaseKey, 4m),
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

    // Through UpdateDamage, not UpgradeValueBy: both write Damage.BaseValue and the later write
    // would erase the other. IsUpgraded is already true here.
    protected override void OnUpgrade()
    {
        DynamicVars[IncreaseKey].UpgradeValueBy(1m);
        UpdateDamage();
    }

    // A downgrade rebuilds Damage from the constants, dropping the growth.
    protected override void AfterDowngraded() => UpdateDamage();

    private void BuffFromPlay(int extraDamage)
    {
        IncreasedDamage += extraDamage;
        UpdateDamage();
    }

    private void UpdateDamage() =>
        CurrentDamage = (IsUpgraded ? UpgradedBaseDamage : BaseDamage) + IncreasedDamage;
}
