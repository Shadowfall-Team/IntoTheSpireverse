using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent;

[Pool(typeof(ShadowSilentCardPool))]
public abstract class ShadowSilentCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    IntoTheSpireverseCard(cost, type, rarity, target, "silent");