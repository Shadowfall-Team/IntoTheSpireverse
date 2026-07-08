using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ui;

public partial class NAmmoCounterReminder : Control
{
    private NAmmoCounter _counter = null!;
    private Player? _player;
    private Tween? _fadeTween;

    public override void _Ready()
    {
        _counter = ResourceLoader.Load<PackedScene>(IntoTheSpireverseResources.AmmoCounterScene).Instantiate<NAmmoCounter>();
        var font = PreloadManager.Cache.GetAsset<Font>(IntoTheSpireverseResources.MegaLabelFont);
        _counter.ApplyFont(font, minSize: 32, maxSize: 32);
        _counter.Modulate = new Color(1, 1, 1, 0);
        AddChild(_counter);
    }

    public void Initialize(Player player)
    {
        _player = player;
        UpdateVisibility();
    }

    public override void _EnterTree()
    {
        AmmoResource.AmmoChanged += OnAmmoChanged;
        CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
    }

    public override void _ExitTree()
    {
        AmmoResource.AmmoChanged -= OnAmmoChanged;
        CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
    }

    private void OnAmmoChanged(PlayerCombatState pcs, int oldVal, int newVal)
    {
        if (_player == null || pcs != _player.PlayerCombatState) return;
        UpdateVisibility();
    }

    private void OnCombatStateChanged(CombatState _) => UpdateVisibility();

    private void UpdateVisibility()
    {
        var shouldShow = _player != null && AmmoResource.CanSpendAmmo(_player)
                         && _player.Creature.CombatState?.CurrentSide == CombatSide.Player;

        if (shouldShow)
            _counter.SetCount(AmmoResource.GetAmmo(_player!));

        _fadeTween?.Kill();
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(_counter, "modulate:a", shouldShow ? 1f : 0f, 0.2f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);
    }
}
