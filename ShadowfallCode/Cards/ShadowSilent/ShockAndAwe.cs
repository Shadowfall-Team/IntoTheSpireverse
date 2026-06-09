using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class ShockAndAwe() : ShadowSilentCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const string StatKey = "Stats";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(StatKey, 2m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromCard<Weight>(false),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ShockAndAweStrengthPower>(
            choiceContext, Owner.Creature, DynamicVars[StatKey].BaseValue,
            Owner.Creature, this);

        await PowerCmd.Apply<ShockAndAweDexterityPower>(
            choiceContext, Owner.Creature, DynamicVars[StatKey].BaseValue,
            Owner.Creature, this);

        var weight = CombatState.CreateCard<Weight>(Owner);
        await CardPileCmd.AddGeneratedCardsToCombat(
            new[] { weight }, PileType.Hand, Owner);
    }

    protected override void OnUpgrade() =>
        DynamicVars[StatKey].UpgradeValueBy(1m);
}