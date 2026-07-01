using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;

/// <summary>
/// Phantom card created once per combat to serve as the CardModel owner-handle
/// when firing Hook.AfterEnergySpent from FireAmmoAction.
/// Never actually added to any pile and never played.
/// </summary>
[Pool(typeof(TokenCardPool))]
public class AmmoVolley() : CustomCardModel(1,
    CardType.Attack,
    CardRarity.Token,
    TargetType.RandomEnemy)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        throw new InvalidOperationException("AmmoVolley is a phantom card and should never actually be played.");
    }
}
