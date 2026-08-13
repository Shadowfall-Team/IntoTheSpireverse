using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Potions;

[Pool(typeof(ShadowSilentPotionPool))]
public class NightmareFuelPotion : IntoTheSpireversePotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target == null || target.Player == null) return;
        AssertValidForTargetedPotion(target);
        var selection = (await CardSelectCmd.FromHand(choiceContext, target.Player, new CardSelectorPrefs(SelectionScreenPrompt, 0, 1), null, this)).FirstOrDefault();
        if (selection != null)
        {
            for (int i = 0; i < DynamicVars.Cards.IntValue; ++i)
            {
                await CardPileCmd.AddGeneratedCardToCombat(selection.CreateClone(), PileType.Hand, target.Player);
            }
        }
    }
}