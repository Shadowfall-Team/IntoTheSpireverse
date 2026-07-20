using BaseLib.Cards.Variables;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;

public sealed class HolyShield() : ShadowSilentCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        // TODO: using persist here is a hack because the GetResultPileTypeForCardPlay function changed in beta
        // using persist gives essentially the same functionality, but it adds a tooltip
        new PersistVar(100)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Muddle),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        void MuddleAfterPlay()
        {
            Played -= MuddleAfterPlay;
            if (Pile?.Type == PileType.Hand)
                IntoTheSpireverseKeywords.ApplyMuddle(this);
        }
        Played += MuddleAfterPlay;
    }

    // protected override (PileType, CardPilePosition) GetResultPileTypeAndPositionForCardPlay()
    // {
    //     (PileType pileType, CardPilePosition cardPilePosition) = base.GetResultPileTypeAndPositionForCardPlay();
    //     return pileType == PileType.Discard ? (PileType.Hand, CardPilePosition.Bottom) : (pileType, cardPilePosition);
    // }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
