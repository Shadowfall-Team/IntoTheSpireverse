using Godot;

namespace Shadowfall;

public static class CardShaderHelper
{
    private const string ShaderPath = "res://rendering/color_adjust.gdshader";

    public static ShaderMaterial CreateMaterial(float h, float s, float v, float r = 1f, float g = 1f, float b = 1f, float contrast = 1f)
    {
        var shader = (Shader)GD.Load<Shader>(ShaderPath).Duplicate();
        var mat = new ShaderMaterial();
        mat.Shader = shader;
        SetParameters(mat, h, s, v, r, g, b, contrast);
        return mat;
    }

    public static void ApplyToPortrait(TextureRect portrait, float h, float s, float v, float r = 1f, float g = 1f, float b = 1f, float contrast = 1f)
    {
        if (portrait.Material is ShaderMaterial existing)
            SetParameters(existing, h, s, v, r, g, b, contrast);
        else
            portrait.Material = CreateMaterial(h, s, v, r, g, b, contrast);
    }

    private static void SetParameters(ShaderMaterial mat, float h, float s, float v, float r, float g, float b, float contrast)
    {
        mat.SetShaderParameter("hue_shift",  h);
        mat.SetShaderParameter("saturation", s);
        mat.SetShaderParameter("value",      v);
        mat.SetShaderParameter("red",        r);
        mat.SetShaderParameter("green",      g);
        mat.SetShaderParameter("blue",       b);
        mat.SetShaderParameter("contrast",   contrast);
    }
}