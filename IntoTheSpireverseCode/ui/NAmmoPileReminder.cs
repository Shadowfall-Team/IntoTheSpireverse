using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ui;

public partial class NAmmoPileReminder : Control
{
    private NAmmoPile _pile = null!;
    private Player? _player;
    private Tween? _fadeTween;

    public override void _Ready()
    {
        _pile = ResourceLoader.Load<PackedScene>(IntoTheSpireverseResources.AmmoPileScene).Instantiate<NAmmoPile>();
        var font = PreloadManager.Cache.GetAsset<Font>(IntoTheSpireverseResources.MegaLabelFont);
        _pile.ApplyFont(font, minSize: 32, maxSize: 32);
        _pile.Modulate = new Color(1, 1, 1, 0);
        AddChild(_pile);
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
        {
            _pile.SetCount(AmmoResource.GetAmmo(_player!));
        }

        var opacity = shouldShow ? 1f : 0f;

        _fadeTween?.Kill();
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(_pile, "modulate:a", opacity, 0.2f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);
    }
}