using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowDefect;

[Pool(typeof(ShadowDefectCardPool))]
public abstract class ShadowDefectCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    IntoTheSpireverseCard(cost, type, rarity, target, "defect");