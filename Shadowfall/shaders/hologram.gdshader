shader_type canvas_item;

uniform vec4 baseColor: source_color = vec4(0.3058, 0.835, 0.960, 1.);
uniform float speed: hint_range(0.0, 1.0) = 0.005;
uniform vec4 linesColor: source_color = vec4(0.633232, 0.910156, 0.555693, 1.);
uniform float linesColorIntensity = 5.;
uniform sampler2D hologramTexture: hint_default_white;
uniform vec2 hologramTextureTiling = vec2(1., 5.);
uniform float edgeBrightness: hint_range(0.0, 1.0) = 0.71;
uniform float uvOffsetY = 0.0;

varying float objectPositionY;

vec2 tilingAndOffset(vec2 uv, vec2 tiling, vec2 offset) {
    return mod(uv * tiling + offset, 1.0);
}

void vertex() {
    objectPositionY = (MODEL_MATRIX * vec4(0.0, 0.0, 0.0, 1.0)).y;
}

void fragment() {
    vec2 offset = vec2(0.0, TIME * speed + uvOffsetY - objectPositionY * 0.01);
    vec2 tiledUV = tilingAndOffset(UV, hologramTextureTiling, offset);

    vec4 noise = texture(hologramTexture, tiledUV);

    vec4 colorLines = linesColor * vec4(vec3(linesColorIntensity), 1.0);
    vec4 emission = colorLines * edgeBrightness * noise;

    vec4 albedo = baseColor;
    float alpha = dot(noise.rgb, vec3(0.2126, 0.7152, 0.0722));
    vec4 hologram;
    hologram.rgb = emission.rgb + (1.0 - emission.rgb) * albedo.rgb;
    hologram.a = emission.a + (1.0 - emission.a) * alpha;
    COLOR = texture(TEXTURE, UV);
    COLOR.rgb = COLOR.rgb + (1.0 - COLOR.rgb) * hologram.rgb;
    COLOR.a = min(COLOR.a, hologram.a);
}
