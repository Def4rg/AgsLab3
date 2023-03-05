#version 330 core

uniform mat4 matrix

in vec2 vPosition;

void main ()
{
	gl_Position = 6 * vec4 (vPosition, 0, 1);
}
