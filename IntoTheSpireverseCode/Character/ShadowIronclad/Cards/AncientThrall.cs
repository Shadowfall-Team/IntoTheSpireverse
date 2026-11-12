using BaseLib.Extensions;
using BaseLib.Utils;
using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;

[Pool(typeof(ShadowIroncladCardPool))]
public sealed class AncientThrall() : ShadowIroncladCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(2m),
        new PowerVar<StrengthPower>(2m),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    protected override bool ShouldGlowRedInternal => true;

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner != Owner) return true;
        if (Pile?.Type != PileType.Hand) return true;
        if (card is AncientThrall or Enthralled) return true;
        if (autoPlayType != AutoPlayType.None) return true;
        return false;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmdCompatibility.Damage(choiceContext, Owner.Creature,
            DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, this, cardPlay);
        
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature, DynamicVars.Power<StrengthPower>().BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Strength.UpgradeValueBy(1m);
}

[HarmonyPatch]
static class EnthralledPatch
{
    [HarmonyPatch(typeof(Enthralled), nameof(Enthralled.ShouldPlay))]
    public static void ShouldPlay(CardModel card, ref bool __result)
    {
        if (!__result && card is AncientThrall)
        {
            __result = true;
        }
    }
}