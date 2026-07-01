using BaseLib.Config.UI;
using Godot;
using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches.Input;

[HarmonyPatch(typeof(NInputManager), nameof(NInputManager._UnhandledKeyInput))]
public static class NInputManagerPatches
{
    public enum CaptureTarget
    {
        None,
        CargoPile,
        Fire
    }

    private record BindingEntry(
        Func<(Key Modifier, Key Key)> Get,
        Action<(Key Modifier, Key Key)> Set,
        Func<NClickableControl?> GetNode);

    // ponytail: add an entry here + a CaptureTarget value to register a new keybind
    private static readonly Dictionary<CaptureTarget, BindingEntry> Bindings = new()
    {
        {
            CaptureTarget.CargoPile, new BindingEntry(
                () => (IntoTheSpireverseConfig.CargoPileModifier, IntoTheSpireverseConfig.CargoPileKey),
                binding => (IntoTheSpireverseConfig.CargoPileModifier, IntoTheSpireverseConfig.CargoPileKey) = binding,
                ModShortcutHelpers.GetCargoPile)
        },
        {
            CaptureTarget.Fire, new BindingEntry(
                () => (IntoTheSpireverseConfig.FireModifier, IntoTheSpireverseConfig.FireKey),
                binding => (IntoTheSpireverseConfig.FireModifier, IntoTheSpireverseConfig.FireKey) = binding,
                ModShortcutHelpers.GetAmmoButton)
        },
    };

    [HarmonyPrefix]
    public static bool UnhandledKeyInputPrefix(InputEvent inputEvent)
    {
        var key = inputEvent as InputEventKey;
        if (key == null || key.IsEcho() || !key.Pressed)
        {
            return _currentlyCapturing == CaptureTarget.None;
        }

        if (_currentlyCapturing != CaptureTarget.None)
        {
            return HandleCapture(key);
        }

        return HandleShortcut(key);
    }

    private static bool IsModifierKey(Key keycode) =>
        keycode is Key.Ctrl or Key.Shift or Key.Alt or Key.Meta;

    private static bool MatchesBinding(InputEventKey key, Key modifierKey, Key keycode)
    {
        if (key.Keycode != keycode) return false;
        return modifierKey switch
        {
            Key.Ctrl => key.CtrlPressed,
            Key.Shift => key.ShiftPressed,
            Key.Alt => key.AltPressed,
            Key.Meta => key.MetaPressed,
            Key.None => key is { CtrlPressed: false, ShiftPressed: false, AltPressed: false, MetaPressed: false },
            _ => false,
        };
    }

    private static Key ModifierFromEvent(InputEventKey key) =>
        key.CtrlPressed ? Key.Ctrl
        : key.ShiftPressed ? Key.Shift
        : key.AltPressed ? Key.Alt
        : key.MetaPressed ? Key.Meta
        : Key.None;

    private static CaptureTarget _currentlyCapturing = CaptureTarget.None;
    private static NConfigButton? _capturingButton;

    public static void StartCapture(CaptureTarget target, NConfigButton button)
    {
        _currentlyCapturing = target;
        _capturingButton = button;
    }

    private static bool HandleCapture(InputEventKey key)
    {
        // Keep listening
        if (IsModifierKey(key.Keycode))
        {
            return false;
        }

        var binding = Bindings[_currentlyCapturing];

        if (key.Keycode == Key.Escape)
        {
            var (modifier, k) = binding.Get();
            _capturingButton?.GetNodeOrNull<Label>("Label")!.Text = KeybindConfigUi.BindingLabel(modifier, k);
        }
        else
        {
            var newModifier = ModifierFromEvent(key);
            binding.Set((newModifier, key.Keycode));
            _capturingButton?.GetNodeOrNull<Label>("Label")!.Text = KeybindConfigUi.BindingLabel(newModifier, key.Keycode);
        }

        _currentlyCapturing = CaptureTarget.None;
        _capturingButton = null;
        return false;
    }

    private static bool HandleShortcut(InputEventKey key)
    {
        foreach (var (_, binding) in Bindings)
        {
            var (modifier, k) = binding.Get();
            if (!MatchesBinding(key, modifier, k)) continue;
            var node = binding.GetNode();
            if (node?.IsVisibleInTree() == true)
            {
                ModShortcutHelpers.OnPressHandler(node);
                ModShortcutHelpers.OnReleaseHandler(node);
            }

            return false;
        }

        return true;
    }
}