using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class Dazzle() : ShadowSilentCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const string MuddleCountKey = "MuddleCount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move),
        new DynamicVar(MuddleCountKey, 1m),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay, false);
        
        var targets = PileType.Hand.GetPile(Owner).Cards
            .Where(IntoTheSpireverseKeywords.CanMuddle)
            .OrderByDescending(c => c.EnergyCost.GetWithModifiers(CostModifiers.All))
            .Take(DynamicVars[MuddleCountKey].IntValue)
            .ToList();

        if (targets.Count == 0)
            return;
        
        IntoTheSpireverseKeywords.ApplyMuddleAll(targets);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars[MuddleCountKey].UpgradeValueBy(1m);
    }
}