shader_type spatial;
render_mode unshaded, blend_mix, depth_prepass_alpha, cull_disabled, specular_disabled;

// textura do sprite
uniform sampler2D sprite_texture : source_color, filter_nearest;

// controles do outline
uniform bool enable_outline = false;
uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float outline_thickness : hint_range(0.0, 10.0) = 1.0;

// velocidade e intensidade do brilho
uniform float glow_speed : hint_range(0.1, 10.0) = 3.0;
uniform float glow_intensity : hint_range(0.0, 2.0) = 0.5;

void fragment() {
    // amostra base
    vec4 col = texture(sprite_texture, UV);

    // default: sem outline
    ALBEDO = col.rgb;
    ALPHA = col.a;

    if (enable_outline) {
        vec2 pixel_size = 1.0 / vec2(textureSize(sprite_texture, 0));

        float hasNeighbor = 0.0;

        // 4 direções
        hasNeighbor += texture(sprite_texture, UV + vec2(pixel_size.x * outline_thickness, 0.0)).a;
        hasNeighbor += texture(sprite_texture, UV + vec2(-pixel_size.x * outline_thickness, 0.0)).a;
        hasNeighbor += texture(sprite_texture, UV + vec2(0.0, pixel_size.y * outline_thickness)).a;
        hasNeighbor += texture(sprite_texture, UV + vec2(0.0, -pixel_size.y * outline_thickness)).a;

        if (col.a < 0.5 && hasNeighbor > 0.0) {
            // brilho pulsante baseado no tempo
            float glow = 0.5 + 0.5 * sin(TIME * glow_speed);
            glow = 1.0 + glow * glow_intensity; // aumenta intensidade
            ALBEDO = outline_color.rgb * glow;
            ALPHA = outline_color.a;
        }
    }
}