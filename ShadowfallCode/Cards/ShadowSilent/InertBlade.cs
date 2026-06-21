using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class InertBlade() : ShadowSilentCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const string StrengthKey = "StrengthGain";

    protected override bool ShouldGlowGoldInternal =>
        EnergyCost.GetWithModifiers(CostModifiers.All) >= 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new CardsVar(3),
        new BlockVar(9m, ValueProp.Move),
        new DynamicVar(StrengthKey, 3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Muddle),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        int cost = EnergyCost.GetWithModifiers(CostModifiers.All);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (cost >= 1)
        {
            await CardPileCmd.Draw(
                choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }

        if (cost >= 2)
        {
            await CreatureCmd.GainBlock(
                Owner.Creature, DynamicVars.Block, cardPlay);
        }

        if (cost >= 3)
        {
            await PowerCmd.Apply<StrengthPower>(
                choiceContext, Owner.Creature,
                DynamicVars[StrengthKey].BaseValue,
                Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Cards.UpgradeValueBy(1m);
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars[StrengthKey].UpgradeValueBy(1m);
    }
}