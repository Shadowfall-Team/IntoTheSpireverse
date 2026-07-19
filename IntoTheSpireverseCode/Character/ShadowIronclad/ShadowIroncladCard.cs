using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad;

[Pool(typeof(ShadowIroncladCardPool))]
public abstract class ShadowIroncladCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    IntoTheSpireverseCard(cost, type, rarity, target, "ironclad");