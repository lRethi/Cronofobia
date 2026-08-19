shader_type spatial;

render_mode unshaded, cull_disabled, blend_add;

uniform sampler2D albedo_texture : source_color, filter_nearest, repeat_disable;
uniform bool glow_enabled = false;
uniform vec4 glow_color : source_color = vec4(1.0, 0.75, 0.2, 1.0);
uniform float glow_strength = 1.0;

void fragment()
{
    vec4 tex = texture(albedo_texture, UV);

    float glow_mask =
        step(0.7, tex.r) *
        step(0.5, tex.g) *
        step(tex.b, 0.4);

    float enabled = float(glow_enabled);
    float alpha = tex.a * glow_mask * enabled;

    ALBEDO = glow_color.rgb;
    EMISSION = glow_color.rgb * glow_strength * enabled;
    ALPHA = alpha;
}