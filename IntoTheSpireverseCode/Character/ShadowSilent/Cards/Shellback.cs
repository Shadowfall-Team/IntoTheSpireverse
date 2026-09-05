using MegaCrit.Sts2.Core.Animation;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Modifications;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards.Colorless;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Modifications;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;


public sealed class Shellback() : ShadowSilentCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string IncreaseKey = "Increase";
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(IncreaseKey, 4m),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(IntoTheSpireverseKeywords.Modify),
            HoverTipFactory.Static(StaticHoverTip.Block),
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.PowerUpAnimDelay);

        var blockCards = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c.Type == CardType.Skill && Modification.CanModify(c) && c.GainsBlock)
            .ToList();

        foreach (var card in blockCards)
        {
            CardModifier.AddModifier<ShellbackModification>(card, DynamicVars[IncreaseKey].IntValue);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars[IncreaseKey].UpgradeValueBy(2m);
    }
}
