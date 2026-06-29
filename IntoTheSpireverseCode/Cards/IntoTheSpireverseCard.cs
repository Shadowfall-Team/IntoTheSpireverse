using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards;


public abstract class IntoTheSpireverseCard(int cost, CardType type, CardRarity rarity, TargetType target, string artFolder) :
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


[Pool(typeof(ShadowIroncladCardPool))]
public abstract class ShadowIroncladCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    IntoTheSpireverseCard(cost, type, rarity, target, "ironclad");


[Pool(typeof(ShadowRegentCardPool))]
public abstract class ShadowRegentCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    IntoTheSpireverseCard(cost, type, rarity, target, "regent");
