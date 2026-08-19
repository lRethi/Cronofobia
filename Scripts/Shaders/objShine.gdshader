shader_type spatial;

render_mode unshaded, cull_disabled, depth_draw_never;

uniform sampler2D albedo_texture : source_color, filter_nearest, repeat_disable;
uniform vec4 glow_base_color : source_color = vec4(0.741, 0.729, 0.592, 1.0);
uniform float glow_tolerance = 0.15;
uniform float glow_strength = 4.0;
uniform bool glow_enabled = false;

void vertex()
{
    MODELVIEW_MATRIX = VIEW_MATRIX * mat4(
        INV_VIEW_MATRIX[0],
        INV_VIEW_MATRIX[1],
        INV_VIEW_MATRIX[2],
        MODEL_MATRIX[3]
    );
}

void fragment()
{
    vec4 tex = texture(albedo_texture, UV);

    float color_distance = distance(
        tex.rgb,
        glow_base_color.rgb
    );

    float glow_mask = 1.0 - smoothstep(
        0.0,
        glow_tolerance,
        color_distance
    );

    float enabled = float(glow_enabled);

    ALBEDO = tex.rgb * glow_strength;
    ALPHA = tex.a * glow_mask * enabled;

    ALPHA_SCISSOR_THRESHOLD = 0.5;
}