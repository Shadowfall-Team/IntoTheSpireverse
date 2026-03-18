using BaseLib.Abstracts;
using BaseLib.Utils;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Potions;

[Pool(typeof(ShadowDefectPotionPool))]
public abstract class ShadowfallPotion : CustomPotionModel;