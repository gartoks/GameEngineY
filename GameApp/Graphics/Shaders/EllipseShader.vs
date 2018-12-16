#version 330 core

uniform mat4 u_viewProjectionMatrix = mat4(1.0);
uniform mat4 u_modelMatrix = mat4(1.0);

layout(location = 0) in vec3 in_position;
layout(location = 2) in vec2 in_localCoords;

out vec2 v_localCoords;

void main() {
	v_localCoords = in_localCoords;
	vec4 pos = vec4(in_position, 1.0);
	gl_Position = u_viewProjectionMatrix * u_modelMatrix * pos;
}