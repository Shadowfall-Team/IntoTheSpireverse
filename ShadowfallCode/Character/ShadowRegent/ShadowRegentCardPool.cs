using BaseLib.Abstracts;
using Godot;

namespace Shadowfall.ShadowfallCode.Character.ShadowRegent;

// TODO impl
public class ShadowRegentCardPool : CustomCardPoolModel
{
    public override string Title { get; }
    public override Color DeckEntryCardColor { get; }
    public override bool IsColorless { get; }
}