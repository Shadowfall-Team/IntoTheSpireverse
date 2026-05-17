using BaseLib.Abstracts;
using BaseLib.Utils;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Character.ShadowDefect;

namespace Shadowfall.ShadowfallCode.Potions;

[Pool(typeof(ShadowDefectPotionPool))]
public abstract class ShadowfallPotion : CustomPotionModel;