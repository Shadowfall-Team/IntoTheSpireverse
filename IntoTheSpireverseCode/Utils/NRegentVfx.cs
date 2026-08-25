using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;

#nullable disable
namespace IntoTheSpireverse.IntoTheSpireverseCode.Utils;

public partial class NRegentVfx : Node
{

  private GpuParticles2D _deathParticlesArm;
  private GpuParticles2D _deathParticlesChest;
  private GpuParticles2D _deathParticlesBack;
  private GpuParticles2D _deathParticlesLeg;
  private GpuParticles2D _deathParticlesLegL;
  private GpuParticles2D _explosionParticles;
  private MegaSprite _weapon;
  private MegaSprite _weapon2;
  private GpuParticles2D _attackParticlesSmall;
  private GpuParticles2D _attackParticlesSmall2;
  private GpuParticles2D _attackParticlesLarge;
  private MegaAnimationState? _weaponAnimState;
  private MegaAnimationState? _weaponAnimState2;
  private int _curWeapon = 1;
  private Node2D _parent;
  private MegaSprite _animController;

  public override void _Ready()
  {
    _parent = GetParent<Node2D>();
    _animController = new MegaSprite((Variant) (GodotObject) _parent);
    _animController.ConnectAnimationEvent(Callable.From(new Action<GodotObject, GodotObject, GodotObject, GodotObject>(OnAnimationEvent)));
    _animController.ConnectAnimationStarted(Callable.From(new Action<GodotObject, GodotObject, GodotObject>(OnAnimationStart)));
    _deathParticlesArm = _parent.GetNode<GpuParticles2D>("SpineArmBone/Particles");
    _deathParticlesChest = _parent.GetNode<GpuParticles2D>("SpineChestBone/Particles");
    _deathParticlesBack = _parent.GetNode<GpuParticles2D>("SpineChestBone/ParticlesBack");
    _deathParticlesLeg = _parent.GetNode<GpuParticles2D>("SpineLegBone/Particles");
    _deathParticlesLegL = _parent.GetNode<GpuParticles2D>("SpineLegBoneL/Particles");
    _explosionParticles = _parent.GetNode<GpuParticles2D>("Explosion");
    _weapon = new MegaSprite(_parent.GetNode("Weapons/WeaponAnim1"));
    _weapon2 = new MegaSprite(_parent.GetNode( "Weapons/WeaponAnim2"));
    this.RunWhenSpineReady(_weapon, animState => _weaponAnimState = animState);
    this.RunWhenSpineReady(_weapon2,animState => _weaponAnimState2 = animState);
    _deathParticlesArm.Emitting = false;
    _deathParticlesChest.Emitting = false;
    _deathParticlesBack.Emitting = false;
    _deathParticlesLeg.Emitting = false;
    _deathParticlesLegL.Emitting = false;
    _explosionParticles.Emitting = false;
  }

  private void OnAnimationEvent(
    GodotObject _,
    GodotObject __,
    GodotObject ___,
    GodotObject spineEvent)
  {
    switch (new MegaEvent((Variant) spineEvent).GetData().GetEventName())
    {
      case "death_particles_start":
        TurnOnDying();
        break;
      case "death_particles_start2":
        TurnOnDying2();
        break;
      case "death_particles_end":
        TurnOffDying();
        break;
      case "explode_dead":
        Explode();
        break;
      case "explode_end":
        DisableExplode();
        break;
      case "attack1":
        Attack();
        break;
    }
  }

  private void TurnOnDying()
  {
    _deathParticlesArm.Restart();
    _deathParticlesLeg.Restart();
    _deathParticlesLegL.Restart();
  }

  private void TurnOnDying2()
  {
    _deathParticlesChest.Restart();
    _deathParticlesBack.Restart();
  }

  private void TurnOffDying()
  {
    _deathParticlesArm.Emitting = false;
    _deathParticlesChest.Emitting = false;
    _deathParticlesBack.Emitting = false;
    _deathParticlesLeg.Emitting = false;
    _deathParticlesLegL.Emitting = false;
  }

  private void Explode() => _explosionParticles.Restart();

  private void DisableExplode() => _explosionParticles.Emitting = false;

  private void Attack()
  {
    if (_curWeapon == 1)
    {
      _weaponAnimState?.SetAnimation("attack", false);
      _curWeapon = 2;
    }
    else
    {
      _weaponAnimState2?.SetAnimation("attack2", false);
      _curWeapon = 1;
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
    if (!(new MegaAnimationState((Variant) animationState).GetCurrentAnimationName() != "die"))
      return;
    DisableExplode();
    TurnOffDying();
  }
}