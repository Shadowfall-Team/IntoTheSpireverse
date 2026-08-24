using System.Reflection.Emit;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Utils;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Platform;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches
{
    [HarmonyPatch(typeof(NHoverTipSet), "Init")]
    public static class ArtHoverTipColorPatch
    {
        private static readonly ShaderMaterial ArtistHoverTipMaterial = ShaderUtils.GenerateHsv(0.2f, 1.35f, 0.9f);

        public static void SetupArtTip(IHoverTip itip, Control tip)
        {
            if (
                itip == null
                || !GodotObject.IsInstanceValid(tip)
                || !ArtistHoverTipUtils.ArtistTipIds.Contains(itip.Id)
            )
            {
                return;
            }

            tip.GetNodeOrNull<CanvasItem?>("%Bg")?.Material = ArtistHoverTipMaterial;

            if (tip.GetNodeOrNull<MegaRichTextLabel?>("%Description") is not { } description)
                return;

            description.MouseFilter = Control.MouseFilterEnum.Stop;

            Node current = description.GetParent();
            while (current is Control parent)
            {
                parent.MouseFilter = Control.MouseFilterEnum.Pass;
                if (parent == NGame.Instance?.HoverTipsContainer)
                    break;
                current = current.GetParent();
            }

            if (!description.IsConnected("meta_clicked", Callable.From<Variant>(OnUrlClicked)))
            {
                description.Connect("meta_clicked", Callable.From<Variant>(OnUrlClicked));
            }
        }

        private static void OnUrlClicked(Variant meta)
        {
            string url = meta.ToString();
            if (!string.IsNullOrWhiteSpace(url) && url.StartsWith("http"))
            {
                PlatformUtil.OpenUrl(url);
            }
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions
        )
        {
            var matcher = new CodeMatcher(instructions);

            // find IHoverTip local
            matcher.MatchStartForward(
                new CodeMatch(i =>
                    i.opcode == OpCodes.Isinst
                    && i.operand?.ToString()?.Contains("HoverTip") == true
                )
            );
            if (matcher.IsInvalid)
                return instructions;
            var tipModelLoad = matcher.InstructionAt(-1);

            // find jump label after IsDebuff check
            matcher.MatchStartForward(
                new CodeMatch(i => i.operand?.ToString()?.Contains("get_IsDebuff") == true)
            );
            if (matcher.IsInvalid)
                return instructions;
            var afterDebuffLabel = (System.Reflection.Emit.Label)matcher.Advance(1).Operand;

            // find the instruction associated with the label
            matcher.MatchStartForward(new CodeMatch(i => i.labels.Contains(afterDebuffLabel)));
            if (matcher.IsInvalid)
                return instructions;

            var tipControlLoad = matcher.Instruction;
            var labels = new List<System.Reflection.Emit.Label>(tipControlLoad.labels);
            tipControlLoad.labels.Clear();

            // insert method call
            return matcher
                .Insert(
                    new CodeInstruction(tipModelLoad.opcode, tipModelLoad.operand).WithLabels(
                        labels
                    ),
                    new CodeInstruction(tipControlLoad.opcode, tipControlLoad.operand),
                    new CodeInstruction(
                        OpCodes.Call,
                        AccessTools.Method(typeof(ArtHoverTipColorPatch), nameof(SetupArtTip))
                    )
                )
                .InstructionEnumeration();
        }
    }
}
