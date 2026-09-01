using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;


public sealed class NaturesDisciple() : ShadowSilentCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    private const string _deviousKey = "Devious";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(1m),
        new DynamicVar(_deviousKey, 0m),
    ];
    
    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Devious];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        IsUpgraded ? HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.DeviousX) : HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Devious),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await IntoTheSpireverseKeywords.ExecuteDevious(choiceContext, Owner, this, DynamicVars[_deviousKey].IntValue, () =>
            PowerCmd.Apply<StrengthPower>(
                choiceContext, Owner.Creature,
                DynamicVars.Power<StrengthPower>().BaseValue,
                Owner.Creature, this)
        );
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars[_deviousKey].UpgradeValueBy(1m);
    }
}