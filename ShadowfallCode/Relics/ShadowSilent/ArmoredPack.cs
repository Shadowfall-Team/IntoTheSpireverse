using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Relics;

  
public class ArmoredPack : ShadowSilentRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override RelicModel? GetUpgradeReplacement()
    {
      return ModelDb.Relic<ArmoredPackUpgrade>();
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4M, ValueProp.Unpowered),
        new CardsVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
      HoverTipFactory.FromCard<Weight>(),
      HoverTipFactory.Static(StaticHoverTip.Block)
    ];

  	public override async Task BeforeCombatStart()
    {
		Flash();
    await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null); 
    }

  public override async Task BeforeHandDraw(
    Player player,
    PlayerChoiceContext choiceContext,
    CombatState combatState)
  {
    if (player != Owner || combatState.RoundNumber != 1)
      return;
    List<CardModel?> cards = new List<CardModel?>();
    for (int index = 0; index < DynamicVars.Cards.IntValue; ++index)
      cards.Add(Owner.Creature.CombatState?.CreateCard<Weight>(Owner));
    await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) cards, PileType.Hand, true);
  }

}