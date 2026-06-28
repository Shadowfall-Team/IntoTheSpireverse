using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public class AmmoVolley() : IntoTheSpireverseCard(1,
    CardType.Attack,
    CardRarity.Token,
    TargetType.RandomEnemy, "regent")
{
    // public override string CustomPortraitPath => $"res://IntoTheSpireverse/images/card_portraits/regent/big/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    public const decimal BaseDamage = 12;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BaseDamage, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        throw new InvalidOperationException("AmmoVolley is a phantom card and should never be played");
    }

    protected override void OnUpgrade()
    {
    }

    public override TargetType TargetType => TargetType.RandomEnemy;
}