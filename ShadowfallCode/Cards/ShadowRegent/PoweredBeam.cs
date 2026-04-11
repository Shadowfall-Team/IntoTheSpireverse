using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Cards.ShadowRegent;

public class PoweredBeam() : ShadowRegentCard(1,
    CardType.Skill,
    CardRarity.Basic,
    TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<AmmoPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<AmmoPower>()
    ];

    protected override void OnUpgrade()
    {
        //TODO: What should upgrade?
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast",
            Owner.Character.CastAnimDelay);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext,
        CardModel card, bool fromHandDraw)
    {
        if (card == this)
        {
            await PowerCmd.Apply<AmmoPower>(
                Owner.Creature,
                DynamicVars[nameof(AmmoPower)].BaseValue,
                Owner.Creature,
                this);
        }
    }
}