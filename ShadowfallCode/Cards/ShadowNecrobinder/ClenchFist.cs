using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class ClenchFist() : ShadowNecrobinderCard(1, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<VulnerablePower>(2),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromKeyword(ShadowfallKeywords.Linger)
    };

    public override bool HasTurnEndInHandEffect => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }

    public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        // VfxCmd.PlayOnCreatureCenter(Owner.Creature, "vfx/vfx_power_up");
        await PowerCmd.Apply<DrawCardsNextTurnPower>(Owner.Creature, 1m, Owner.Creature, this);
        // GetResultPileType();
        ModifyCardPlayResultPileTypeAndPosition(this, false, new ResourceInfo{EnergySpent = 0, EnergyValue = 0, StarsSpent = 0, StarValue = 0}, PileType.Draw, CardPilePosition.Random);
        // await PowerCmd.Apply<Linger>(Owner.Creature, 1m, Owner.Creature)
        await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Random);
    }
}
