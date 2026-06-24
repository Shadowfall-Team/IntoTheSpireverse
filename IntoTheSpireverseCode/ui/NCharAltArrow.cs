using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ui;

public partial class NCharAltArrow : NGoldArrowButton
{
    public ICharacterSelectButtonDelegate ClickDelegate;

    public List<CharacterModel> Characters = new();

    private int _currentAltIndex = -1;
    private Tween? _portraitTween;
    private float _originalY;

    public override void _Ready()
    {
        _originalY = (GetParent() as NCharacterSelectButton)?.Position.Y ?? Position.Y;
        base._Ready();
    }

    protected override void OnPress()
    {
        if (DoPress()) return;

        base.OnPress();
    }

    public bool DoPress()
    {
        if (GetParent() is not NCharacterSelectButton parent) return true;

        _currentAltIndex = (_currentAltIndex + 1) % Characters.Count;
        var character = Characters[_currentAltIndex];

        parent.Init(character, ClickDelegate);

        var portraitContainer = parent.GetNode<Control>("MarginContainer");
        if (portraitContainer != null)
        {
            BounceUp(portraitContainer, 20.0f, 0.15f);
        }

        if (parent.IsSelected)
        {
            ClickDelegate.SelectCharacter(parent, character);
        }

        return false;
    }

    private void BounceUp(Control node, float amount = 20.0f, float duration = 0.3f)
    {
        _portraitTween?.Kill();
        _portraitTween = CreateTween();
        _portraitTween.SetParallel(true);

        _portraitTween.TweenProperty(node, "position:y", _originalY - amount, duration)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Expo);

        _portraitTween.TweenProperty(node, "position:y", _originalY, duration)
            .SetDelay(duration)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Expo);

        _portraitTween.TweenProperty(node, "modulate:a", 0.0f, duration)
            .SetEase(Tween.EaseType.Out);
        _portraitTween.TweenProperty(node, "modulate:a", 1.0f, duration)
            .SetDelay(duration)
            .SetEase(Tween.EaseType.In);
    }
}