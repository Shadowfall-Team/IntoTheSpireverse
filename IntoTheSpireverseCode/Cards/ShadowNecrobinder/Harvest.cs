using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowNecrobinder;

public sealed class Harvest() : ShadowNecrobinderCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    
    // Rarity and the upgrade were not specified in the draft document, so consider them placeholder
    
    private const string _increaseKey = "Increase";
    private Decimal _extraDamageFromLinger;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar(_increaseKey, 3m),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Linger)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        IntoTheSpireverseKeywords.Linger
    ];
    
    private Decimal ExtraDamageFromLinger
    {
        get => _extraDamageFromLinger;
        set
        {
            AssertMutable();
            _extraDamageFromLinger = value;
        }
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars[_increaseKey].UpgradeValueBy(1m);
    }
    
    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Damage.BaseValue += ExtraDamageFromLinger;
    }

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        int triggers = LingerHelper.GetTriggerCount(this);
        for (int i = 0; i < triggers; i++)
        {
            DynamicVars.Damage.BaseValue += DynamicVars[_increaseKey].BaseValue;
            ExtraDamageFromLinger += DynamicVars[_increaseKey].BaseValue;
            LingerHelper.PendingLingerRedirect.Add(this);
            await LingerHelper.NotifyLingerTriggered(this, choiceContext);
        }
    }
}
