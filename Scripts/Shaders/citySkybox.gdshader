shader_type sky;

uniform vec3 sky_morning_top : source_color = vec3(0.18, 0.22, 0.30);
uniform vec3 sky_morning_bottom : source_color = vec3(0.72, 0.58, 0.48);

uniform vec3 sky_day_top : source_color = vec3(0.20, 0.27, 0.36);
uniform vec3 sky_day_bottom : source_color = vec3(0.62, 0.61, 0.56);

uniform vec3 sky_evening_top : source_color = vec3(0.15, 0.11, 0.18);
uniform vec3 sky_evening_bottom : source_color = vec3(0.95, 0.38, 0.16);

uniform vec3 sky_twilight_top : source_color = vec3(0.055, 0.035, 0.085);
uniform vec3 sky_twilight_bottom : source_color = vec3(0.52, 0.11, 0.045);

uniform vec3 sky_night_top : source_color = vec3(0.004, 0.006, 0.018);
uniform vec3 sky_night_bottom : source_color = vec3(0.025, 0.012, 0.028);

uniform vec3 city_day_color : source_color = vec3(0.045, 0.044, 0.048);
uniform vec3 city_evening_color : source_color = vec3(0.075, 0.030, 0.022);
uniform vec3 city_night_color : source_color = vec3(0.008, 0.008, 0.015);

uniform vec3 window_color : source_color = vec3(1.0, 0.34, 0.10);
uniform vec3 window_cold_color : source_color = vec3(0.24, 0.42, 0.78);

uniform vec3 haze_morning_color : source_color = vec3(0.56, 0.46, 0.39);
uniform vec3 haze_day_color : source_color = vec3(0.36, 0.35, 0.33);
uniform vec3 haze_evening_color : source_color = vec3(0.80, 0.22, 0.08);
uniform vec3 haze_night_color : source_color = vec3(0.055, 0.012, 0.025);

uniform vec3 sun_direction = vec3(0.0, 1.0, 0.0);

uniform float morning_factor = 0.0;
uniform float day_factor = 1.0;
uniform float evening_factor = 0.0;
uniform float twilight_factor = 0.0;
uniform float night_factor = 0.0;
uniform float artificial_light_factor = 0.0;

uniform float city_density = 1.0;
uniform float city_height = 0.30;
uniform float horizon_height = 0.015;
uniform float horizon_haze = 0.20;

uniform float window_density = 0.65;
uniform float window_brightness = 2.0;

uniform float antenna_density = 0.10;
uniform float star_density = 0.025;
uniform float star_brightness = 0.25;

uniform float city_night_visibility = 0.58;
uniform float city_twilight_visibility = 0.82;

float hash(float value)
{
	return fract(sin(value * 12.9898) * 43758.5453);
}

float hash_vec2(vec2 value)
{
	return fract(
		sin(
			dot(
				value,
				vec2(127.1, 311.7)
			)
		) * 43758.5453
	);
}

float noise(vec2 value)
{
	vec2 cell = floor(value);
	vec2 local = fract(value);

	local = local * local * (3.0 - 2.0 * local);

	float a = hash_vec2(cell);
	float b = hash_vec2(cell + vec2(1.0, 0.0));
	float c = hash_vec2(cell + vec2(0.0, 1.0));
	float d = hash_vec2(cell + vec2(1.0, 1.0));

	return mix(
		mix(a, b, local.x),
		mix(c, d, local.x),
		local.y
	);
}

float get_building_type(float id)
{
	return hash(id + 47.19);
}

float get_building_width(float id)
{
	float type = get_building_type(id);
	float random_width = hash(id + 17.31);

	if (type < 0.20)
		return mix(0.62, 0.96, random_width);

	if (type < 0.42)
		return mix(0.38, 0.82, random_width);

	if (type < 0.60)
		return mix(0.68, 0.98, random_width);

	if (type < 0.76)
		return mix(0.42, 0.92, random_width);

	if (type < 0.90)
		return mix(0.34, 0.70, random_width);

	return mix(0.22, 0.48, random_width);
}

float get_building_height(float id)
{
	float type = get_building_type(id);
	float random_height = hash(id);

	if (type < 0.20)
		return mix(0.07, 0.18, random_height);

	if (type < 0.42)
		return mix(0.16, 0.32, random_height);

	if (type < 0.60)
		return mix(0.10, 0.24, random_height);

	if (type < 0.76)
		return mix(0.24, 0.46, random_height);

	if (type < 0.90)
		return mix(0.34, 0.68, random_height);

	return mix(0.70, 1.00, random_height);
}

float get_building_profile(float id, float local_position)
{
	float type = get_building_type(id);

	if (type >= 0.20 && type < 0.42)
	{
		float roof_center =
			1.0 -
			abs(local_position - 0.5) * 2.0;

		return mix(
			0.82,
			1.0,
			roof_center
		);
	}

	if (type >= 0.42 && type < 0.60)
	{
		float roof = noise(
			vec2(
				id * 0.17,
				local_position * 3.0
			)
		);

		return mix(
			0.82,
			1.0,
			roof
		);
	}

	if (type >= 0.76 && type < 0.90)
	{
		float upper_section =
			smoothstep(
				0.68,
				0.92,
				local_position
			);

		return mix(
			1.0,
			0.90,
			upper_section
		);
	}

	return 1.0;
}

float get_city_layer(
	vec2 horizontal_direction,
	float scale,
	float offset,
	float height_multiplier
)
{
	float angle =
		atan(
			horizontal_direction.y,
			horizontal_direction.x
		);

	float normalized_angle =
		angle / 6.28318530718 + 0.5;

	float position =
		normalized_angle *
		scale +
		offset;

	float id = floor(position);
	float local_position = fract(position);

	float width =
		get_building_width(id);

	float inside =
		1.0 -
		step(
			width,
			local_position
		);

	float height =
		get_building_height(id);

	float profile =
		get_building_profile(
			id,
			local_position
		);

	return height *
		height_multiplier *
		profile *
		inside;
}

float get_industrial_layer(
	vec2 horizontal_direction,
	float scale,
	float offset
)
{
	float angle =
		atan(
			horizontal_direction.y,
			horizontal_direction.x
		);

	float normalized_angle =
		angle / 6.28318530718 + 0.5;

	float position =
		normalized_angle *
		scale +
		offset;

	float id = floor(position);
	float local_position = fract(position);

	float type =
		get_building_type(id);

	float width =
		get_building_width(id);

	float inside =
		1.0 -
		step(
			width,
			local_position
		);

	float industrial =
		step(0.60, type) *
		step(type, 0.76);

	float base_height =
		mix(
			0.10,
			0.24,
			hash(id + 21.7)
		);

	float roof_variation =
		noise(
			vec2(
				id * 0.31,
				local_position * 4.0
			)
		);

	float height =
		base_height *
		mix(
			0.78,
			1.18,
			roof_variation
		);

	float chimney_seed =
		hash(id + 83.17);

	float chimney =
		step(0.55, chimney_seed) *
		smoothstep(
			0.78,
			0.94,
			local_position
		) *
		0.12;

	return (
		height +
		chimney
	) *
	industrial *
	inside;
}

float get_windows(
	vec3 direction,
	float scale,
	float offset,
	float layer_strength
)
{
	vec2 horizontal_direction =
		normalize(
			vec2(
				direction.x,
				direction.z
			)
		);

	float angle =
		atan(
			horizontal_direction.y,
			horizontal_direction.x
		);

	float normalized_angle =
		angle / 6.28318530718 + 0.5;

	float position =
		normalized_angle *
		scale +
		offset;

	float id = floor(position);
	float local_x = fract(position);

	float building_width =
		get_building_width(id);

	if (local_x > building_width)
		return 0.0;

	float building_height =
		get_building_height(id) *
		city_height *
		layer_strength;

	float profile =
		get_building_profile(
			id,
			local_x
		);

	float top =
		horizon_height +
		building_height *
		profile;

	if (direction.y > top)
		return 0.0;

	float normalized_height =
		1.0 -
		max(direction.y, 0.0) /
		max(top, 0.001);

	float type =
		get_building_type(id);

	float floors =
		mix(
			3.0,
			20.0,
			hash(id + 73.21)
		);

	if (type < 0.20)
	{
		floors =
			mix(
				1.0,
				4.0,
				hash(id + 74.21)
			);
	}

	if (type >= 0.60 && type < 0.76)
	{
		floors =
			mix(
				2.0,
				7.0,
				hash(id + 75.21)
			);
	}

	float columns =
		mix(
			3.0,
			13.0,
			hash(id + 13.72)
		);

	if (type < 0.20)
	{
		columns =
			mix(
				2.0,
				5.0,
				hash(id + 14.72)
			);
	}

	if (type >= 0.60 && type < 0.76)
	{
		columns =
			mix(
				3.0,
				8.0,
				hash(id + 15.72)
			);
	}

	float floor_position =
		normalized_height *
		floors;

	float base_floor_id =
		floor(floor_position);

	float floor_local =
		fract(floor_position);

	float column_position =
		local_x *
		columns;

	float base_column_id =
		floor(column_position);

	float column_local =
		fract(column_position);

	float cell_seed =
		id +
		base_floor_id * 17.31 +
		base_column_id * 43.71;

	float random_window =
		hash(cell_seed);

	float enabled =
		step(
			1.0 - window_density,
			random_window
		);

	float facade_zone =
		noise(
			vec2(
				id * 0.73 +
				base_column_id * 0.37,
				base_floor_id * 0.61
			)
		);

	float zone_mask =
		smoothstep(
			0.22,
			0.68,
			facade_zone
		);

	float cluster_random =
		hash(
			id +
			base_floor_id * 91.37 +
			floor(base_column_id / 2.0) * 53.17
		);

	float cluster_mask =
		step(
			0.25,
			cluster_random
		);

	float horizontal_jitter =
		(
			hash(cell_seed + 17.41) -
			0.5
		) * 0.24;

	float vertical_jitter =
		(
			hash(cell_seed + 31.72) -
			0.5
		) * 0.24;

	float jittered_column =
		column_local +
		horizontal_jitter;

	float jittered_floor =
		floor_local +
		vertical_jitter;

	float window_width =
		mix(
			0.42,
			0.72,
			hash(cell_seed + 51.83)
		);

	float window_height =
		mix(
			0.38,
			0.70,
			hash(cell_seed + 71.24)
		);

	float horizontal_gap =
		step(
			0.5 - window_width * 0.5,
			jittered_column
		) *
		step(
			jittered_column,
			0.5 + window_width * 0.5
		);

	float vertical_gap =
		step(
			0.5 - window_height * 0.5,
			jittered_floor
		) *
		step(
			jittered_floor,
			0.5 + window_height * 0.5
		);

	float irregularity =
		noise(
			vec2(
				local_x * columns * 0.55 +
				id * 0.19,
				normalized_height * floors * 0.65 +
				id * 0.31
			)
		);

	float irregular_mask =
		smoothstep(
			0.25,
			0.75,
			irregularity
		);

	float final_cluster =
		mix(
			cluster_mask,
			cluster_mask * zone_mask,
			0.65
		);

	final_cluster *=
		mix(
			0.72,
			1.0,
			irregular_mask
		);

	if (type < 0.20)
		final_cluster *= 0.72;

	if (type >= 0.60 && type < 0.76)
		final_cluster *= 0.48;

	return enabled *
		horizontal_gap *
		vertical_gap *
		final_cluster;
}

float get_antennas(
	vec3 direction,
	float scale,
	float offset
)
{
	vec2 horizontal_direction =
		normalize(
			vec2(
				direction.x,
				direction.z
			)
		);

	float angle =
		atan(
			horizontal_direction.y,
			horizontal_direction.x
		);

	float normalized_angle =
		angle / 6.28318530718 + 0.5;

	float position =
		normalized_angle *
		scale +
		offset;

	float id = floor(position);
	float local_position = fract(position);

	float random_value =
		hash(id + 55.13);

	float enabled =
		step(
			1.0 - antenna_density,
			random_value
		);

	float center =
		max(
			1.0 -
			abs(local_position - 0.5) *
			25.0,
			0.0
		);

	float top =
		smoothstep(
			0.68,
			0.94,
			direction.y
		);

	return enabled *
		center *
		top;
}

float get_stars(vec3 direction)
{
	if (direction.y < 0.08)
		return 0.0;

	vec2 coordinates =
		normalize(
			vec2(
				direction.x,
				direction.z
			)
		) *
		500.0;

	float value =
		noise(
			coordinates +
			direction.y *
			37.0
		);

	return step(
		1.0 - star_density,
		value
	);
}

void sky()
{
	vec3 direction =
		normalize(EYEDIR);

	vec3 actual_sun_direction =
		normalize(sun_direction);

	float vertical =
		clamp(
			direction.y * 0.5 + 0.5,
			0.0,
			1.0
		);

	float sky_vertical =
		smoothstep(
			0.0,
			0.82,
			vertical
		);

	vec3 morning_sky =
		mix(
			sky_morning_bottom,
			sky_morning_top,
			sky_vertical
		);

	vec3 day_sky =
		mix(
			sky_day_bottom,
			sky_day_top,
			sky_vertical
		);

	vec3 evening_sky =
		mix(
			sky_evening_bottom,
			sky_evening_top,
			sky_vertical
		);

	vec3 twilight_sky =
		mix(
			sky_twilight_bottom,
			sky_twilight_top,
			sky_vertical
		);

	vec3 night_sky =
		mix(
			sky_night_bottom,
			sky_night_top,
			sky_vertical
		);

	vec3 sky_color =
		night_sky;

	sky_color =
		mix(
			sky_color,
			morning_sky,
			morning_factor
		);

	sky_color =
		mix(
			sky_color,
			day_sky,
			day_factor
		);

	sky_color =
		mix(
			sky_color,
			evening_sky,
			evening_factor
		);

	sky_color =
		mix(
			sky_color,
			twilight_sky,
			twilight_factor
		);

	float horizon =
		1.0 -
		smoothstep(
			0.0,
			horizon_haze,
			abs(direction.y)
		);

	vec3 haze_color =
		haze_night_color;

	haze_color =
		mix(
			haze_color,
			haze_morning_color,
			morning_factor
		);

	haze_color =
		mix(
			haze_color,
			haze_day_color,
			day_factor
		);

	haze_color =
		mix(
			haze_color,
			haze_evening_color,
			evening_factor
		);

	sky_color +=
		haze_color *
		horizon *
		(
			0.18 +
			horizon * 0.24
		);

	float horizon_glow =
		pow(
			max(
				1.0 -
				abs(direction.y),
				0.0
			),
			3.5
		);

	vec3 warm_horizon =
		vec3(
			1.0,
			0.18,
			0.035
		);

	float sunset_glow =
		evening_factor *
		horizon_glow *
		1.45;

	sunset_glow +=
		twilight_factor *
		horizon_glow *
		0.85;

	sky_color +=
		warm_horizon *
		sunset_glow;

	float sun_dot =
		max(
			dot(
				direction,
				actual_sun_direction
			),
			0.0
		);

	float sun_disc =
		smoothstep(
			0.99935,
			0.99982,
			sun_dot
		);

	float sun_halo =
		pow(
			sun_dot,
			180.0
		);

	float sun_visibility =
		max(
			evening_factor,
			day_factor
		);

	sun_visibility =
		max(
			sun_visibility,
			morning_factor
		);

	sun_visibility *=
		1.0 -
		night_factor;

	vec3 sun_color =
		vec3(
			1.0,
			0.74,
			0.42
		);

	sun_color =
		mix(
			sun_color,
			vec3(
				1.0,
				0.30,
				0.08
			),
			evening_factor
		);

	sky_color +=
		sun_color *
		sun_disc *
		2.8 *
		sun_visibility;

	sky_color +=
		sun_color *
		sun_halo *
		0.65 *
		sun_visibility;

	float atmospheric_red =
		smoothstep(
			0.0,
			0.52,
			1.0 - abs(direction.y)
		);

	sky_color +=
		vec3(
			0.16,
			0.018,
			0.008
		) *
		atmospheric_red *
		evening_factor *
		0.25;

	vec2 horizontal_direction =
		normalize(
			vec2(
				direction.x,
				direction.z
			)
		);

	float city_near =
		get_city_layer(
			horizontal_direction,
			95.0 * city_density,
			0.0,
			1.0
		);

	float city_mid =
		get_city_layer(
			horizontal_direction,
			145.0 * city_density,
			31.7,
			0.68
		);

	float city_far =
		get_city_layer(
			horizontal_direction,
			210.0 * city_density,
			71.3,
			0.42
		);

	float industrial =
		get_industrial_layer(
			horizontal_direction,
			125.0 * city_density,
			52.8
		);

	float skyline =
		max(
			city_near,
			max(
				city_mid,
				city_far
			)
		);

	skyline =
		max(
			skyline,
			industrial
		);

	float skyline_height =
		horizon_height +
		skyline *
		city_height;

	float skyline_mask =
		1.0 -
		smoothstep(
			skyline_height,
			skyline_height + 0.018,
			direction.y
		);

	float city_visibility =
		max(
			day_factor,
			night_factor *
			city_night_visibility
		);

	city_visibility =
		max(
			city_visibility,
			evening_factor *
			0.78
		);

	city_visibility =
		max(
			city_visibility,
			twilight_factor *
			city_twilight_visibility
		);

	city_visibility =
		clamp(
			city_visibility,
			0.0,
			1.0
		);

	vec3 city_color =
		mix(
			city_night_color,
			city_day_color,
			city_visibility
		);

	city_color =
		mix(
			city_color,
			city_evening_color,
			evening_factor *
			0.72
		);

	city_color =
		mix(
			city_color,
			vec3(
				0.16,
				0.035,
				0.025
			),
			twilight_factor *
			0.30
		);

	sky_color =
		mix(
			sky_color,
			city_color,
			skyline_mask
		);

	float windows_near =
		get_windows(
			direction,
			95.0 * city_density,
			0.0,
			1.0
		);

	float windows_mid =
		get_windows(
			direction,
			145.0 * city_density,
			31.7,
			0.68
		);

	float windows_far =
		get_windows(
			direction,
			210.0 * city_density,
			71.3,
			0.42
		);

	float windows =
		max(
			windows_near,
			max(
				windows_mid * 0.65,
				windows_far * 0.30
			)
		);

	float window_factor =
		max(
			artificial_light_factor,
			evening_factor * 0.42
		);

	window_factor =
		max(
			window_factor,
			twilight_factor * 0.70
		);

	windows *=
		skyline_mask *
		window_factor;

	float cold_windows =
		windows *
		(
			1.0 -
			step(
				0.72,
				noise(
					horizontal_direction *
					80.0
				)
			)
		);

	float warm_windows =
		windows -
		cold_windows;

	sky_color +=
		window_color *
		warm_windows *
		window_brightness;

	sky_color +=
		window_cold_color *
		cold_windows *
		window_brightness *
		0.65;

	float antennas_near =
		get_antennas(
			direction,
			95.0 * city_density,
			0.0
		);

	float antennas_mid =
		get_antennas(
			direction,
			145.0 * city_density,
			31.7
		);

	float antennas =
		max(
			antennas_near,
			antennas_mid *
			0.5
		);

	sky_color +=
		window_color *
		antennas *
		artificial_light_factor *
		1.5;

	float stars =
		get_stars(direction);

	stars *=
		night_factor *
		(1.0 - twilight_factor);

	sky_color +=
		vec3(
			stars *
			star_brightness
		);

	COLOR = sky_color;
}