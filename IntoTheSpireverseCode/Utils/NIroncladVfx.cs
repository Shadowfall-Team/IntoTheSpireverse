
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

#nullable disable
namespace IntoTheSpireverse.IntoTheSpireverseCode.Utils;

public partial class NIroncladVfx : Node
{

  private static readonly StringName _step = new StringName("step");
  private Vector2 _slashStepBase;
  private ShaderMaterial? _slashShaderMat;
  private Tween? _tween;
  private Node2D _parent;
  private MegaSprite _megaSprite;
  private TextureRect _eyeFireTex;

  public override void _Ready()
  {
    _parent = GetParent<Node2D>();
    _slashShaderMat = new MegaSlotNode((Variant) (GodotObject) _parent.GetNode((NodePath) "SlashVfxSlot")).GetNormalMaterial() as ShaderMaterial;
    if (_slashShaderMat != null) _slashStepBase = (Vector2)_slashShaderMat.GetShaderParameter(_step);
    _eyeFireTex = _parent.GetNode<TextureRect>((NodePath) "EyeSlot/EyeFire");
    _megaSprite = new MegaSprite((Variant) (GodotObject) _parent);
    _megaSprite.ConnectAnimationEvent(Callable.From(new Action<GodotObject, GodotObject, GodotObject, GodotObject>(OnAnimationEvent)));
    _megaSprite.ConnectAnimationStarted(Callable.From(new Action<GodotObject, GodotObject, GodotObject>(OnAnimationStart)));
    OnClearVfx();
  }

  private void OnAnimationEvent(
    GodotObject _,
    GodotObject __,
    GodotObject ___,
    GodotObject spineEvent)
  {
    switch (new MegaEvent((Variant) spineEvent).GetData().GetEventName())
    {
      case "heavy_slash_start":
        OnHeavySlash();
        break;
      case "attack_slash_start":
        OnAttackSlash();
        break;
      case "cast_eyes_start":
        OnCastEyes();
        break;
      case "clear_vfx":
        OnClearVfx();
        break;
    }
  }

  /// <summary>
  /// Check if we want to make sure we turn off any vfx between animations. We have to do this if the animation that
  /// is supposed to turn off the vfx is interrupted early.
  /// </summary>
  private void OnAnimationStart(
    GodotObject spineSprite,
    GodotObject animationState,
    GodotObject trackEntry)
  {
    if (!(new MegaAnimationState(animationState).GetCurrentAnimationName() != "cast"))
      return;
    OnClearVfx();
  }

  private void OnHeavySlash()
  {
    _slashShaderMat?.SetShaderParameter(_step, _slashStepBase);
    _tween?.Kill();
    _tween = CreateTween().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
    _tween.TweenProperty(_slashShaderMat, "shader_parameter/step", new Vector2(1f, 1.02f), 0.3499999940395355);
  }

  private void OnAttackSlash()
  {
    _slashShaderMat?.SetShaderParameter(_step, _slashStepBase);
    _tween?.Kill();
    _tween = CreateTween().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
    Vector2 finalVal = new Vector2(1f, 1.02f);
    _tween.TweenInterval(0.15000000596046448);
    _tween.TweenProperty(_slashShaderMat, "shader_parameter/step", finalVal, 0.20000000298023224);
  }

  private void OnCastEyes() => _eyeFireTex.Visible = true;

  private void OnClearVfx() => _eyeFireTex.Visible = false;

  public override void _ExitTree() => _tween?.Kill();
}