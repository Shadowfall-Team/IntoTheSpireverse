
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class NSpineAutoPlayer : Node
{
  public override void _Ready()
  {
	MegaSprite sprite = new MegaSprite((Variant) (GodotObject) this.GetParent());
	this.RunWhenSpineReady(sprite, (Action<MegaAnimationState>) (animState =>
	{
	  IReadOnlyList<string> animationNames = sprite.GetSkeleton().GetData().GetAnimationNames();
	  if (animationNames.Count != 1)
		throw new InvalidOperationException($"{nameof (NSpineAutoPlayer)}'s parent's skeleton data must have exactly 1 animation. This has {animationNames.Count}.");
	  animState.SetAnimation(animationNames[0]);
	}));
  }
  
  public override void _Process(double delta)
  {
  }
}
