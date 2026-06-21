using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public sealed class Ouroborous() : ShadowSilentCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9m, ValueProp.Move),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EnergyHoverTip,
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        var card = (await CardSelectCmd.FromHand(
            choiceContext, Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
            null, this)).FirstOrDefault();

        if (card == null)
            return;

        int cost;
        if (card.EnergyCost.CostsX && Owner.PlayerCombatState != null)
            cost = Owner.PlayerCombatState.Energy;
        else
            cost = card.EnergyCost.GetWithModifiers(CostModifiers.All);

        await CardCmd.Exhaust(choiceContext, card);

        if (cost > 0)
            await PlayerCmd.GainEnergy((decimal)cost, Owner);
    }

    protected override void OnUpgrade() =>
        RemoveKeyword(CardKeyword.Exhaust);
}