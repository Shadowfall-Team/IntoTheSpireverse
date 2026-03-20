using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Relics;

  
public class ArmoredPack : ShadowSilentRelic
{

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DexterityPower>(1M),
        new CardsVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
      HoverTipFactory.FromCard<Weight>(),
      HoverTipFactory.FromPower<DexterityPower>()
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
  {
    ArmoredPack armoredPack = this;
    if (room is not CombatRoom)
      return;
    armoredPack.Flash();
    await PowerCmd.Apply<DexterityPower>(armoredPack.Owner.Creature, armoredPack.DynamicVars.Dexterity.BaseValue, armoredPack.Owner.Creature, null);
  }

  public override async Task BeforeHandDraw(
    Player player,
    PlayerChoiceContext choiceContext,
    CombatState combatState)
  {
    ArmoredPack armoredPack = this;
    if (player != armoredPack.Owner || combatState.RoundNumber != 1)
      return;
    List<CardModel?> cards = new List<CardModel?>();
    for (int index = 0; index < armoredPack.DynamicVars.Cards.IntValue; ++index)
      cards.Add(armoredPack.Owner.Creature.CombatState?.CreateCard<Weight>(armoredPack.Owner));
    await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) cards, PileType.Hand, true);
  }

}