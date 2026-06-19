using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using Shadowfall.ShadowfallCode.Character;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Shadowfall.ShadowfallCode.Cards;


public abstract class ShadowfallCard(int cost, CardType type, CardRarity rarity, TargetType target, string artFolder) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string? CustomPortraitPath
    {
        get
        {
            var name = Id.Entry.RemovePrefix().ToLowerInvariant();
            var path = $"res://{MainFile.ModId}/images/card_portraits/{artFolder}/big/{name}.png";
            return ResourceLoader.Exists(path) ? path : base.CustomPortraitPath;
        }
    }
}

[Pool(typeof(ShadowDefectCardPool))]
public abstract class ShadowDefectCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    ShadowfallCard(cost, type, rarity, target, "defect");

[Pool(typeof(ShadowIroncladCardPool))]
public abstract class ShadowIroncladCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    ShadowfallCard(cost, type, rarity, target, "ironclad");

[Pool(typeof(ShadowSilentCardPool))]
public abstract class ShadowSilentCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    ShadowfallCard(cost, type, rarity, target, "silent");

[Pool(typeof(ShadowNecrobinderCardPool))]
public abstract class ShadowNecrobinderCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    ShadowfallCard(cost, type, rarity, target, "necrobinder");

[Pool(typeof(ShadowRegentCardPool))]
public abstract class ShadowRegentCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    ShadowfallCard(cost, type, rarity, target, "regent");
