using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowNecrobinder;

public sealed class PureAura() : ShadowNecrobinderCard(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string _lingerDamageKey = "LingerDamage";

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(15m, ValueProp.Move),
        new DamageVar(_lingerDamageKey, 4m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Linger)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        IntoTheSpireverseKeywords.Linger
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5m);
        DynamicVars[_lingerDamageKey].UpgradeValueBy(2m);
    }

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        int triggers = LingerHelper.GetTriggerCount(this);
        for (int i = 0; i < triggers; i++)
        {
            await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies,
                (DamageVar)DynamicVars[_lingerDamageKey], Owner.Creature);
            await LingerHelper.NotifyLingerTriggered(this, choiceContext);
        }
    }
}
