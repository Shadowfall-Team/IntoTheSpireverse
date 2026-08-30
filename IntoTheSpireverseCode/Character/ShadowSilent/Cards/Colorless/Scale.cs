using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public sealed class Scale() : ShadowColorlessCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Scale];

    private Decimal CurrentBlock;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateInHand(
        Player owner,
        int count,
        ICombatState combatState,
        Player? creator = null)
    {
        if (count == 0)
            return Array.Empty<CardModel>();
        if (CombatManager.Instance.IsOverOrEnding)
            return Array.Empty<CardModel>();
        List<CardModel> scales = new List<CardModel>();
        for (int index = 0; index < count; ++index)
            scales.Add(combatState.CreateCard<Scale>(owner));
        await CardPileCmd.AddGeneratedCardsToCombat(scales, PileType.Hand, creator ?? owner);
        return scales;
    }
    
    public void AddBlock(Decimal amount)
    {
        BlockVar block = DynamicVars.Block;
        block.BaseValue += amount;
        CurrentBlock += amount;
    }
    
    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Block.BaseValue += CurrentBlock;
    }
}
