using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Commands;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowRegent;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public class Hyperdrive() : ShadowRegentCard(-1,
    CardType.Skill,
    CardRarity.Token,
    TargetType.Self)
{
    public override string CustomPortraitPath => $"res://IntoTheSpireverse/images/card_portraits/regent/big/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        
            [
                new PowerVar<ShardsPower>(3)
            ]
    ;
    

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Unplayable];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ShardsPower>()
    ];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext,
        CardModel card, bool fromHandDraw)
    {
        if (card == this)
        {
            await PowerCmd.Apply<ShardsPower>(new ThrowingPlayerChoiceContext(),
                Owner.Creature,
                DynamicVars[nameof(ShardsPower)].BaseValue * await GeneratePlayCount(CombatState, null), Owner.Creature, null);
        }
    }


    protected override void OnUpgrade()
    {
        DynamicVars[nameof(ShardsPower)].UpgradeValueBy(1);
    }
}
