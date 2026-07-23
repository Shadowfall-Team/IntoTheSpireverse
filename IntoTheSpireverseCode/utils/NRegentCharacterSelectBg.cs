
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace IntoTheSpireverse.IntoTheSpireverseCode.utils;

public partial class NRegentCharacterSelectBg : Control
{
  private MegaSprite _spineController;
  private Control _sphereGuardianHover;
  private Control _decaHover;
  private Control _sentryHover;
  private Control _sneckoHover;
  private Control _cultistHover;
  private Control _shapesHover;
  private Control _amogusHover;

  public override void _Ready()
  {
    _spineController = new MegaSprite((Variant) (GodotObject) GetNode((NodePath) "SpineSprite"));
    _sphereGuardianHover = GetNode<Control>((NodePath) "SphereGuardianHover");
    _sphereGuardianHover.Connect(Control.SignalName.MouseEntered, Callable.From((Action) (() => SetSkin("spheric guardian constellation"))));
    _sphereGuardianHover.Connect(Control.SignalName.MouseExited, Callable.From((Action) (() => SetSkin("normal"))));
    _decaHover = GetNode<Control>((NodePath) "DecaHover");
    _decaHover.Connect(Control.SignalName.MouseEntered, Callable.From((Action) (() => SetSkin("deca outline"))));
    _decaHover.Connect(Control.SignalName.MouseExited, Callable.From((Action) (() => SetSkin("normal"))));
    _sentryHover = GetNode<Control>((NodePath) "SentryHover");
    _sentryHover.Connect(Control.SignalName.MouseEntered, Callable.From((Action) (() => SetSkin("sentry constellation"))));
    _sentryHover.Connect(Control.SignalName.MouseExited, Callable.From((Action) (() => SetSkin("normal"))));
    _sneckoHover = GetNode<Control>((NodePath) "SneckoHover");
    _sneckoHover.Connect(Control.SignalName.MouseEntered, Callable.From((Action) (() => SetSkin("snecko constellation"))));
    _sneckoHover.Connect(Control.SignalName.MouseExited, Callable.From((Action) (() => SetSkin("normal"))));
    _cultistHover = GetNode<Control>((NodePath) "CultistHover");
    _cultistHover.Connect(Control.SignalName.MouseEntered, Callable.From((Action) (() => SetSkin("cultist constellation"))));
    _cultistHover.Connect(Control.SignalName.MouseExited, Callable.From((Action) (() => SetSkin("normal"))));
    _shapesHover = GetNode<Control>((NodePath) "ShapesHover");
    _shapesHover.Connect(Control.SignalName.MouseEntered, Callable.From((Action) (() => SetSkin("shapes constellation"))));
    _shapesHover.Connect(Control.SignalName.MouseExited, Callable.From((Action) (() => SetSkin("normal"))));
    _amogusHover = GetNode<Control>((NodePath) "AmogusHover");
    _amogusHover.Connect(Control.SignalName.MouseEntered, Callable.From((Action) (() => SetSkin("amogus constellation"))));
    _amogusHover.Connect(Control.SignalName.MouseExited, Callable.From((Action) (() => SetSkin("normal"))));
  }

  private void SetSkin(string skinName)
  {
    MegaSkeleton skeleton = _spineController.GetSkeleton();
    if (skeleton == null)
      return;
    skeleton.SetSkin(skeleton.GetData().FindSkin(skinName));
    skeleton.SetSlotsToSetupPose();
  }
}
