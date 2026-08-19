shader_type canvas_item;

group_uniforms Primary;
uniform vec4 primary_source : source_color = vec4(0.15, 0.15, 0.15, 1.0);
uniform vec4 primary_target : source_color = vec4(0.8, 0.15, 0.15, 1.0);

group_uniforms Secondary;
uniform vec4 secondary_source : source_color = vec4(0.2, 0.4, 1.0, 1.0);
uniform vec4 secondary_target : source_color = vec4(1.0, 0.8, 0.2, 1.0);

group_uniforms Accent;
uniform vec4 accent_source : source_color = vec4(0.2, 0.8, 0.2, 1.0);
uniform vec4 accent_target : source_color = vec4(0.7, 0.2, 1.0, 1.0);

uniform float hue_tolerance = 0.03;
uniform float saturation_tolerance = 0.30;

vec3 rgb_to_hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);

    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1e-10;

    return vec3(
        abs(q.z + (q.w - q.y) / (6.0 * d + e)),
        d / (q.x + e),
        q.x
    );
}

vec3 hsv_to_rgb(vec3 c)
{
    vec3 rgb = clamp(abs(mod(c.x * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);

    rgb = rgb * rgb * (3.0 - 2.0 * rgb);

    return c.z * mix(vec3(1.0), rgb, c.y);
}

float hue_distance(float a, float b)
{
    float d = abs(a - b);
    return min(d, 1.0 - d);
}

void apply_swap(inout vec3 hsv, vec3 source_rgb, vec3 target_rgb)
{
    vec3 source = rgb_to_hsv(source_rgb);
    vec3 target = rgb_to_hsv(target_rgb);

    if (
        hue_distance(hsv.x, source.x) < hue_tolerance &&
        abs(hsv.y - source.y) < saturation_tolerance
    )
    {
        hsv.x = target.x;
        hsv.y *= max(target.y, 0.001);
        hsv.y = clamp(hsv.y, 0.0, 1.0);
    }
}

void fragment()
{
    vec4 tex = texture(TEXTURE, UV);

    vec3 hsv = rgb_to_hsv(tex.rgb);

    apply_swap(hsv, primary_source.rgb, primary_target.rgb);
    apply_swap(hsv, secondary_source.rgb, secondary_target.rgb);
    apply_swap(hsv, accent_source.rgb, accent_target.rgb);

    COLOR = vec4(hsv_to_rgb(hsv), tex.a);
}