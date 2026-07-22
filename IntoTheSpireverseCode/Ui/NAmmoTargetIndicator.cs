using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ui;

/// <summary>
/// Crosshair pinned to an enemy's health bar while an Ammo shot would hit that enemy.
/// Sizing and the pulse animation live in ammo_target_indicator.tscn; this script only
/// decides when the crosshair is shown and where it sits.
/// </summary>
public partial class NAmmoTargetIndicator : TextureRect
{
    // Horizontal offset from the bar's right edge to the crosshair's left edge.
    // Negative pulls the crosshair back over the end of the bar.
    private const float HorizontalGap = -30f;

    private NCreature _creature = null!;
    private Control _anchor = null!;

    public static NAmmoTargetIndicator Create(NCreature creature, Control anchor)
    {
        var indicator = ResourceLoader
            .Load<PackedScene>(IntoTheSpireverseResources.AmmoTargetIndicatorScene)
            .Instantiate<NAmmoTargetIndicator>();
        indicator._creature = creature;
        indicator._anchor = anchor;
        return indicator;
    }

    public override void _EnterTree()
    {
        AmmoResource.AmmoChanged += OnAmmoChanged;
        AmmoResource.LastAttackTargetChanged += OnLastAttackTargetChanged;
        CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
    }

    public override void _ExitTree()
    {
        AmmoResource.AmmoChanged -= OnAmmoChanged;
        AmmoResource.LastAttackTargetChanged -= OnLastAttackTargetChanged;
        CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
    }

    public override void _Ready() => UpdateVisibility();

    private void OnAmmoChanged(PlayerCombatState pcs, int oldVal, int newVal) => UpdateVisibility();

    private void OnLastAttackTargetChanged(PlayerCombatState pcs, Creature? target) => UpdateVisibility();

    private void OnCombatStateChanged(CombatState state) => UpdateVisibility();

    private void UpdateVisibility()
    {
        Visible = ShouldShow();
        if (Visible) UpdatePosition();
    }

    private bool ShouldShow()
    {
        var combatState = _creature.Entity.CombatState;
        if (combatState == null) return false;

        // With a single target there's no ambiguity about where Ammo goes.
        if (combatState.HittableEnemies.Count() <= 1) return false;

        var player = LocalContext.GetMe(combatState);
        if (player == null) return false;
        if (AmmoResource.GetAmmo(player) <= 0) return false;
        if (player.Creature.HasPower<MassMunitionPower>()) return false;

        return AmmoResource.GetLastAttackTarget(player) == _creature.Entity;
    }

    /// <summary>
    /// TopLevel means Position is global, so the anchor's screen rect drives it directly
    /// and no ancestor transform, clipping, or layout can move it out of view. The scene
    /// sets PivotOffset to the crosshair's centre, so the pulse scales in place and this
    /// only has to place the unscaled rect: left edge HorizontalGap past the bar's right
    /// edge, vertically centred on the bar.
    ///
    /// Only needs to run when the crosshair is shown: the health bar moves during the
    /// creature's spawn-in animation and is fixed after that, and the crosshair cannot be
    /// visible during the spawn.
    /// </summary>
    private void UpdatePosition()
    {
        var rect = _anchor.GetGlobalRect();
        GlobalPosition = new Vector2(
            rect.Position.X + rect.Size.X + HorizontalGap,
            rect.Position.Y + rect.Size.Y * 0.5f - Size.Y * 0.5f);
    }
}
