using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowNecrobinder;

[Pool(typeof(ShadowNecrobinderCardPool))]
public abstract class ShadowNecrobinderCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    IntoTheSpireverseCard(cost, type, rarity, target, "necrobinder");