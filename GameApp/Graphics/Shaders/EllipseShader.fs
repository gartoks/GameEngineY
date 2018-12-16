#version 330 core
#define PI 3.14159265

uniform vec4 u_color = vec4(1.0);
uniform float u_innerRadius = 0.0;
uniform float u_startAngle = 0.0;
uniform float u_sweepAngle = 1.0;

in vec2 v_localCoords;

layout(location = 0) out vec4 out_color;

void main() {
	float innerRadius = max(0, min(1, u_innerRadius));
	float startAngle = max(0, min(1, u_startAngle));
	float sweepAngle = max(0, min(1, u_sweepAngle));

	float angle = 0.5 * (1.0 + atan(v_localCoords.y, v_localCoords.x) / PI) + 0.5;
	angle = mod(angle, 1.0);
	float radius = length(v_localCoords);

	float angle2 = mod(angle - startAngle, 1.0);
	float a = max(0, angle2 - sweepAngle);
	a = 1.0 - ceil(a);

	float len = max(0, 0.5 - radius) * 2.0;
	float inner = len - 1.0 + innerRadius;

	float cRaw = ceil(len) - ceil(inner);

	vec4 c = vec4(cRaw * a);
	out_color = u_color * c;
}