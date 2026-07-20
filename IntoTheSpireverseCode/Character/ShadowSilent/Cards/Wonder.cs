using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class Wonder() : ShadowSilentCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string MuddleCountKey = "MuddleCount";
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
        new DynamicVar(MuddleCountKey, 1m),
        new EnergyVar(2),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var muddledCards = await IntoTheSpireverseKeywords.ApplyMuddleFromHandSelection(
            choiceContext,
            Owner,
            this,
            DynamicVars[MuddleCountKey].IntValue
        );
        foreach (var card in muddledCards)
            if (GetEffectiveCost(card, card.Owner) >= DynamicVars.Energy.IntValue)
            {
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);
            }
    }

    private static int GetEffectiveCost(CardModel card, Player owner)
    {
        if (card.EnergyCost.CostsX)
            return owner.PlayerCombatState?.Energy ?? 0;
        return card.EnergyCost.GetWithModifiers(CostModifiers.All);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}