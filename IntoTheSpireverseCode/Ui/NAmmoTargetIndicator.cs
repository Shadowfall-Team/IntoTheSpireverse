using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ui;

public partial class NAmmoTargetIndicator : TextureRect
{
    private const float IndicatorScale = 0.35f;

    // Horizontal offset from the bar's right edge to the crosshair's left edge.
    // Negative pulls the crosshair back over the end of the bar.
    private const float HorizontalGap = -30f;

    private const float PulseFrequency = 2f;
    private const float ScaleMin = 1.0f;
    private const float ScaleMax = 1.1f;
    private const float OpacityMin = 0.7f;
    private const float OpacityMax = 0.9f;

    private NCreature _creature = null!;
    private Control _anchor = null!;
    private float _pulseTime;

    public static NAmmoTargetIndicator Create(NCreature creature, Control anchor)
    {
        return new NAmmoTargetIndicator
        {
            _creature = creature,
            _anchor = anchor,
            Texture = ResourceLoader.Load<Texture2D>(IntoTheSpireverseResources.AmmoIndicatorTexture),
            // IgnoreSize lets Size + StretchMode control the rendered dimensions;
            // the default KeepSize would pin it to the texture's native size.
            ExpandMode = ExpandModeEnum.IgnoreSize,
            StretchMode = StretchModeEnum.KeepAspectCentered,
            MouseFilter = MouseFilterEnum.Ignore,
            TopLevel = true,
            ZIndex = 100,
            Visible = false
        };
    }

    public override void _Ready()
    {
        Size = Texture.GetSize() * IndicatorScale;
        UpdatePosition();
    }

    public override void _Process(double delta)
    {
        var shouldShow = ShouldShow();
        if (shouldShow != Visible)
        {
            Visible = shouldShow;
            _pulseTime = 0f;
        }

        if (!shouldShow) return;

        _pulseTime += (float)delta * PulseFrequency;
        var t = (Mathf.Sin(_pulseTime) + 1f) * 0.5f;
        Scale = Vector2.One * Mathf.Lerp(ScaleMin, ScaleMax, t);
        Modulate = new Color(1, 1, 1, Mathf.Lerp(OpacityMin, OpacityMax, t));

        // Position after scaling so the pulse stays centered (see UpdatePosition).
        UpdatePosition();
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
    /// and no ancestor transform, clipping, or layout can move it out of view. We pin the
    /// crosshair's center (rather than its top-left) so the pulse scales symmetrically:
    /// GlobalPosition sets the top-left, which grows down-right under Scale, so we offset
    /// it by half the *scaled* size to hold the center steady. At rest the left edge sits
    /// HorizontalGap past the bar's right edge, vertically centered on the bar.
    /// </summary>
    private void UpdatePosition()
    {
        var rect = _anchor.GetGlobalRect();
        var center = new Vector2(
            rect.Position.X + rect.Size.X + HorizontalGap + Size.X * 0.5f,
            rect.Position.Y + rect.Size.Y * 0.5f);
        GlobalPosition = center - Size * Scale * 0.5f;
    }
}
