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
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics;

  
public class ArmoredPackUpgrade : ShadowSilentRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DexterityPower>(3M),
        new CardsVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
      HoverTipFactory.FromCard<Weight>(), 
      HoverTipFactory.FromPower<DexterityPower>()
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
  {
    if (room is not CombatRoom) 
      return;
    Flash();
    await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.Dexterity.BaseValue, Owner.Creature, null);
  }

  public override async Task BeforeHandDraw(
    Player player,
    PlayerChoiceContext choiceContext,
    ICombatState combatState)
  {
    if (player != Owner || combatState.RoundNumber != 1)
      return;
    List<CardModel?> cards = new List<CardModel?>();
    for (int index = 0; index < DynamicVars.Cards.IntValue; ++index)
      cards.Add(Owner.Creature.CombatState?.CreateCard<Weight>(Owner));
    await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) cards, PileType.Hand, Owner);
  }

}