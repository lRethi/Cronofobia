shader_type spatial;

render_mode unshaded, fog_disabled, cull_disabled, depth_test_disabled, depth_draw_never, blend_mix;

uniform sampler2D depth_texture : hint_depth_texture, repeat_disable, filter_nearest;

uniform vec3 fog_color : source_color = vec3(0.42, 0.44, 0.45);
uniform float density : hint_range(0.0, 0.1, 0.001) = 0.012;
uniform float fog_height : hint_range(-20.0, 20.0, 0.1) = 0.5;
uniform float height_falloff : hint_range(0.1, 30.0, 0.1) = 5.0;
uniform float max_distance : hint_range(10.0, 500.0, 1.0) = 180.0;

uniform float noise_scale : hint_range(0.001, 0.2, 0.001) = 0.025;
uniform float noise_strength : hint_range(0.0, 1.0, 0.01) = 0.25;
uniform vec3 wind_direction = vec3(0.8, 0.0, 0.25);
uniform float wind_speed : hint_range(0.0, 1.0, 0.001) = 0.015;

uniform vec3 sun_color : source_color = vec3(1.0, 0.48, 0.18);
uniform vec3 sun_direction = vec3(0.0, 0.4, -1.0);
uniform float sun_scattering : hint_range(0.0, 5.0, 0.01) = 0.5;
uniform float scattering_anisotropy : hint_range(-0.9, 0.9, 0.01) = 0.35;

const int STEPS = 6;

float hash3(vec3 p)
{
    p = fract(p * 0.1031);
    p += dot(p, p.yzx + 33.33);
    return fract((p.x + p.y) * p.z);
}

float noise3(vec3 p)
{
    vec3 i = floor(p);
    vec3 f = fract(p);

    f = f * f * (3.0 - 2.0 * f);

    float n000 = hash3(i);
    float n100 = hash3(i + vec3(1.0, 0.0, 0.0));
    float n010 = hash3(i + vec3(0.0, 1.0, 0.0));
    float n110 = hash3(i + vec3(1.0, 1.0, 0.0));

    float n001 = hash3(i + vec3(0.0, 0.0, 1.0));
    float n101 = hash3(i + vec3(1.0, 0.0, 1.0));
    float n011 = hash3(i + vec3(0.0, 1.0, 1.0));
    float n111 = hash3(i + vec3(1.0, 1.0, 1.0));

    float nx00 = mix(n000, n100, f.x);
    float nx10 = mix(n010, n110, f.x);
    float nx01 = mix(n001, n101, f.x);
    float nx11 = mix(n011, n111, f.x);

    float nxy0 = mix(nx00, nx10, f.y);
    float nxy1 = mix(nx01, nx11, f.y);

    return mix(nxy0, nxy1, f.z);
}

float phase_hg(float cos_theta, float g)
{
    float g2 = g * g;

    float denominator = pow(
        max(1.0 + g2 - 2.0 * g * cos_theta, 0.0001),
        1.5
    );

    return (1.0 - g2) / (4.0 * PI * denominator);
}

void vertex()
{
    POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment()
{
    float depth = texture(depth_texture, SCREEN_UV).r;

    float fog_amount = 0.0;
    vec3 fog_result = fog_color;

    if (depth < 0.99999)
    {
        vec3 ndc = vec3(SCREEN_UV, depth) * 2.0 - 1.0;

        vec4 view_position =
            INV_PROJECTION_MATRIX *
            vec4(ndc, 1.0);

        view_position.xyz /= view_position.w;

        float scene_distance = max(-view_position.z, 0.0);

        if (scene_distance > 0.001)
        {
            vec4 world_position4 =
                INV_VIEW_MATRIX *
                vec4(view_position.xyz, 1.0);

            world_position4.xyz /= world_position4.w;

            vec3 surface_world = world_position4.xyz;

            vec3 ray_world =
                normalize(
                    surface_world -
                    CAMERA_POSITION_WORLD
                );

            float travel_distance =
                min(
                    scene_distance,
                    max_distance
                );

            float step_length =
                travel_distance /
                float(STEPS);

            vec3 light_direction =
                normalize(sun_direction);

            float view_light_dot =
                dot(
                    -ray_world,
                    light_direction
                );

            float phase =
                phase_hg(
                    view_light_dot,
                    scattering_anisotropy
                );

            float transmittance = 1.0;
            vec3 accumulated = vec3(0.0);

            for (int i = 0; i < STEPS; i++)
            {
                float step_position =
                    (float(i) + 0.5) /
                    float(STEPS);

                float distance_along_ray =
                    step_position *
                    travel_distance;

                vec3 sample_position =
                    CAMERA_POSITION_WORLD +
                    ray_world *
                    distance_along_ray;

                float height_density =
                    exp(
                        -(
                            sample_position.y -
                            fog_height
                        ) /
                        height_falloff
                    );

                height_density =
                    clamp(
                        height_density,
                        0.0,
                        1.0
                    );

                vec3 noise_position =
                    sample_position *
                    noise_scale +
                    wind_direction *
                    TIME *
                    wind_speed;

                float noise =
                    noise3(noise_position);

                float noise_modifier =
                    mix(
                        1.0 - noise_strength,
                        1.0 + noise_strength,
                        noise
                    );

                float local_density =
                    density *
                    height_density *
                    noise_modifier;

                float extinction =
                    1.0 -
                    exp(
                        -local_density *
                        step_length
                    );

                vec3 scattering =
                    fog_color +
                    sun_color *
                    phase *
                    sun_scattering;

                accumulated +=
                    transmittance *
                    extinction *
                    scattering;

                transmittance *=
                    1.0 -
                    extinction;
            }

            fog_amount =
                1.0 -
                transmittance;

            if (fog_amount > 0.0001)
            {
                fog_result =
                    accumulated /
                    fog_amount;
            }
        }
    }

    ALBEDO = fog_result;
    ALPHA = fog_amount;
}