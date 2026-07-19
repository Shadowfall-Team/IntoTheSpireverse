using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent;

[Pool(typeof(ShadowRegentCardPool))]
public abstract class ShadowRegentCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    IntoTheSpireverseCard(cost, type, rarity, target, "regent");