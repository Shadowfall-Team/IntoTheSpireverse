using Godot;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ui;

/// <summary>
/// Pulses the parent Control's scale and opacity while it is visible. The parent's
/// PivotOffset must be its centre (set in the scene) so the scale stays centred and
/// the parent's positioning logic doesn't have to compensate for it.
/// </summary>
public partial class NPulseEffect : Node
{
    [Export] public float Frequency = 2f;
    [Export] public float ScaleMin = 1.0f;
    [Export] public float ScaleMax = 1.1f;
    [Export] public float OpacityMin = 0.7f;
    [Export] public float OpacityMax = 0.9f;

    private Control _target = null!;
    private float _time;

    public override void _Ready()
    {
        _target = GetParent<Control>();
        _target.VisibilityChanged += OnTargetVisibilityChanged;
        OnTargetVisibilityChanged();
    }

    public override void _ExitTree()
    {
        _target.VisibilityChanged -= OnTargetVisibilityChanged;
    }

    public override void _Process(double delta)
    {
        _time += (float)delta * Frequency;
        Apply();
    }

    private void OnTargetVisibilityChanged()
    {
        // IsVisibleInTree, not Visible: an ancestor hiding the target (the creature's state
        // display fading out) also fires this, and the pulse should stop for that too.
        // Restart the pulse each time it reappears so it always starts from the same point.
        _time = 0f;
        SetProcess(_target.IsVisibleInTree());
        Apply();
    }

    private void Apply()
    {
        var t = (Mathf.Sin(_time) + 1f) * 0.5f;
        _target.Scale = Vector2.One * Mathf.Lerp(ScaleMin, ScaleMax, t);
        _target.Modulate = new Color(1, 1, 1, Mathf.Lerp(OpacityMin, OpacityMax, t));
    }
}
