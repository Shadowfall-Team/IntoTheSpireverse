using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Utils;

public partial class NSpineAutoPlayer : Node
{
    public override void _Ready()
    {
        MegaSprite sprite = new MegaSprite((Variant)GetParent());
        this.RunWhenSpineReady(sprite, (animState =>
        {
            IReadOnlyList<string> animationNames = sprite.GetSkeleton().GetData().GetAnimationNames();
            if (animationNames.Count != 1)
                throw new InvalidOperationException($"{nameof(NSpineAutoPlayer)}'s parent's skeleton data must have exactly 1 animation. This has {animationNames.Count}.");
            animState.SetAnimation(animationNames[0]);
        }));
    }

    public override void _Process(double delta)
    {
    }
}
