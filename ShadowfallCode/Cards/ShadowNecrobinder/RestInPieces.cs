using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Keywords;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class RestInPieces() : ShadowNecrobinderCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private Decimal _additionalHits;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2m, ValueProp.Move),
        new RepeatVar(4),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Linger)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        ShadowfallKeywords.Linger
    ];

    private Decimal AdditionalHits
    {
        get => _additionalHits;
        set
        {
            AssertMutable();
            _additionalHits = value;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(2m);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Repeat.BaseValue += AdditionalHits;
    }

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        int triggers = LingerHelper.GetTriggerCount(this);
        for (int i = 0; i < triggers; i++)
        {
            DynamicVars.Repeat.BaseValue += 2m;
            AdditionalHits += 2m;
            await LingerHelper.NotifyLingerTriggered(this, choiceContext);
        }
    }
}
