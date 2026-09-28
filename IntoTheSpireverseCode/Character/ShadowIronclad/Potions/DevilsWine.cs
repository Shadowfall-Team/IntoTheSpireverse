using BaseLib.Abstracts;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Modifications;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Potions;

[Pool(typeof(ShadowIroncladPotionPool))]
public class DevilsWine : IntoTheSpireversePotion
{
    private const string HpLossKey = "HpLoss";
    private const string ReplayKey = "Replay";

    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar(ReplayKey, 1m),
        new DynamicVar(HpLossKey, 2m),
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Modify),
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var modifiable = PileType.Hand.GetPile(Owner).Cards.Where(Modification.CanModify).ToList();
        if (modifiable.Count == 0) return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var card = (await CardSelectCmd.FromSimpleGrid(choiceContext, modifiable, Owner, prefs)).FirstOrDefault();
        if (card == null) return;

        card.BaseReplayCount += DynamicVars[ReplayKey].IntValue;
        CardModifier.AddModifier<DevilsWineModification>(card, DynamicVars[HpLossKey].IntValue);
    }
}
