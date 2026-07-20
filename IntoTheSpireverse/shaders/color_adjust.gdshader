shader_type canvas_item;
// ─────────────────────────────────────────────
//  Color Adjustment Shader  –  Godot 4.x
//  Works on Sprite2D, TextureRect, etc.
// ─────────────────────────────────────────────

// ── RGB Channels ──────────────────────────────
uniform float red   : hint_range(0.0, 2.0) = 1.0;  // 1.0 = unchanged
uniform float green : hint_range(0.0, 2.0) = 1.0;
uniform float blue  : hint_range(0.0, 2.0) = 1.0;

// ── RGB Offset (additive shift, –1 … +1) ──────
uniform float red_offset   : hint_range(-1.0, 1.0) = 0.0;
uniform float green_offset : hint_range(-1.0, 1.0) = 0.0;
uniform float blue_offset  : hint_range(-1.0, 1.0) = 0.0;

// ── Alpha ─────────────────────────────────────
uniform float alpha : hint_range(0.0, 1.0) = 1.0;

// ── HSV ───────────────────────────────────────
uniform float hue_shift  : hint_range(0.0, 2.0) = 1.0;  // 1.0 = no shift
uniform float saturation : hint_range(0.0, 5.0) = 1.0;  // 1.0 = unchanged
uniform float value      : hint_range(0.0, 2.0) = 1.0;  // 1.0 = unchanged

// ── Extra ─────────────────────────────────────
uniform float contrast : hint_range(0.0, 3.0) = 1.0;
uniform float gamma    : hint_range(0.1, 3.0) = 1.0;
uniform vec4  tint     : source_color = vec4(1.0);

// ─────────────────────────────────────────────
//  YIQ matrices
// ─────────────────────────────────────────────
const mat3 RGB_TO_YIQ = mat3(
    vec3(0.2989,  0.5959,  0.2115),
    vec3(0.5870, -0.2774, -0.5229),
    vec3(0.1140, -0.3216,  0.3114));

// ─────────────────────────────────────────────
//  Vertex – capture modulate so COLOR works
// ─────────────────────────────────────────────
varying vec4 modulate_color;

void vertex() {
    modulate_color = COLOR;
}

// ─────────────────────────────────────────────
//  Fragment
// ─────────────────────────────────────────────
void fragment() {
    vec4 col = texture(TEXTURE, UV);

    // 1. RGB channel multiply + additive offset
    col.rgb *= vec3(red, green, blue);
    col.rgb += vec3(red_offset, green_offset, blue_offset);

    // 2. Convert to YIQ
    col.rgb = RGB_TO_YIQ * col.rgb;

    // 3. Hue shift (1.0 = no shift, maps to 0 radians)
    float hue = mix(0.0, 6.283185, 1.0 - hue_shift);
    float sin_hue = sin(hue);
    float cos_hue = cos(hue);
    mat3 hue_mat = mat3(
        vec3(1.0,     0.0,      0.0),
        vec3(0.0, cos_hue, -sin_hue),
        vec3(0.0, sin_hue,  cos_hue));
    col.rgb *= hue_mat;

    // 4. Saturation
    col.rgb = mat3(
        vec3(1.0,        0.0,        0.0),
        vec3(0.0, saturation,        0.0),
        vec3(0.0,        0.0, saturation)) * col.rgb;

    // 5. Value (brightness)
    col.rgb = mix(vec3(0.0), col.rgb, value);

    // 6. Convert back to RGB
    col.rgb = inverse(RGB_TO_YIQ) * col.rgb;

    // 7. Contrast (pivot at 0.5)
    col.rgb = (col.rgb - 0.5) * contrast + 0.5;

    // 8. Gamma correction
    col.rgb = pow(max(col.rgb, vec3(0.0)), vec3(gamma));

    // 9. Tint multiply
    col *= tint;

    // 10. Alpha
    col.a *= alpha;

    COLOR = clamp(col, 0.0, 1.0) * modulate_color;
}
