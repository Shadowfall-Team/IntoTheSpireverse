using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;


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