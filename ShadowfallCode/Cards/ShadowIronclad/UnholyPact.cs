using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Keywords;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Cards.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class UnholyPact() : ShadowIroncladCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string BloodbondKey = "Bloodbond";
    private const string ResolveKey = "Resolve";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(BloodbondKey, 5m),
        new ShadowfallKeywords.GloryVar(4m),
        new DynamicVar(ResolveKey, 3m),
        new HpLossVar(1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BloodbondPower>(),
        ShadowfallKeywords.GloryHoverTipDynamic(DynamicVars[ShadowfallKeywords.GloryVar.defaultName]),
        HoverTipFactory.FromPower<ResolvePower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await PowerCmd.Apply<BloodbondPower>(
            cardPlay.Target, DynamicVars[BloodbondKey].BaseValue,
            Owner.Creature, this);

        if (ShadowfallKeywords.IsGloryTriggered(this))
        {
            await PowerCmd.Apply<ResolvePower>(
                Owner.Creature, DynamicVars[ResolveKey].BaseValue,
                Owner.Creature, this);

            await CreatureCmd.Damage(choiceContext, Owner.Creature,
                DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars[BloodbondKey].UpgradeValueBy(3m);
}