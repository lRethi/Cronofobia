shader_type spatial;
render_mode unshaded, blend_mix, depth_prepass_alpha, cull_disabled, specular_disabled;

uniform sampler2D sprite_texture : source_color, filter_nearest;

uniform bool enable_outline = false;
uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float outline_thickness : hint_range(0.0, 10.0) = 1.0;

uniform float glow_speed : hint_range(0.1, 10.0) = 3.0;
uniform float glow_intensity : hint_range(0.0, 2.0) = 0.5;

void fragment() {
    vec4 col = texture(sprite_texture, UV);

    ALBEDO = col.rgb;
    ALPHA = col.a;

    if (enable_outline) {
        vec2 pixel_size = 1.0 / vec2(textureSize(sprite_texture, 0));

        float cornerAlpha = 0.0;

        cornerAlpha += texture(sprite_texture, vec2(0.0, 0.0)).a;
        cornerAlpha += texture(sprite_texture, vec2(1.0, 0.0)).a;
        cornerAlpha += texture(sprite_texture, vec2(0.0, 1.0)).a;
        cornerAlpha += texture(sprite_texture, vec2(1.0, 1.0)).a;

        bool opaqueTexture = cornerAlpha > 3.5;

        float glow = 0.5 + 0.5 * sin(TIME * glow_speed);
        glow = 1.0 + glow * glow_intensity;

        if (opaqueTexture) {
            float distanceToEdge = min(
                min(UV.x, 1.0 - UV.x),
                min(UV.y, 1.0 - UV.y)
            );

            float thicknessUV = max(
                pixel_size.x,
                pixel_size.y
            ) * outline_thickness;

            if (distanceToEdge <= thicknessUV) {
                ALBEDO = outline_color.rgb * glow;
                ALPHA = outline_color.a;
            }
        } else {
            float hasNeighbor = 0.0;

            hasNeighbor += texture(sprite_texture, UV + vec2(pixel_size.x * outline_thickness, 0.0)).a;
            hasNeighbor += texture(sprite_texture, UV + vec2(-pixel_size.x * outline_thickness, 0.0)).a;
            hasNeighbor += texture(sprite_texture, UV + vec2(0.0, pixel_size.y * outline_thickness)).a;
            hasNeighbor += texture(sprite_texture, UV + vec2(0.0, -pixel_size.y * outline_thickness)).a;

            if (col.a < 0.5 && hasNeighbor > 0.0) {
                ALBEDO = outline_color.rgb * glow;
                ALPHA = outline_color.a;
            }
        }
    }
}