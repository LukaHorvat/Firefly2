﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Facilities
{
	public class DefaultShaders
	{
		public static string VertexShaderSource = @"
#version 130

uniform samplerBuffer buffer;
uniform mat4 camera;
uniform mat4 window;

in vec2 vertex_position;
in vec4 vertex_color;
in vec2 vertex_texcoords;
in float index;

out vec4 fragment_color;
out vec2 fragment_texcoords;

vec2 getRow(int offset)
{
	return vec2(texelFetch(buffer, offset).x, texelFetch(buffer, offset + 1).x);
}

mat2 getMatrix(int offset)
{
	return mat2(getRow(offset),	getRow(offset + 2));
}

vec2 getPosition(int offset)
{
	return getRow(offset + 4);
}

float get(int offset)
{
	return texelFetch(buffer, offset).x;
}

void main()
{
	int i = int(index * 32768) * 6;
//	vec2 position = getPosition(i);
	mat4 model = mat4(	vec4(get(i), get(i + 1), 0, 0),
						vec4(get(i + 2), get(i + 3), 0, 0),
						vec4(0, 0, 1, 0),
						vec4(get(i + 4), get(i + 5), 0, 1));
//	mat4 model = mat4(getMatrix(i));
//	model[2][2] = 1.0;
//	model[3][3] = 1.0;
//	model[3][0] = position.x;
//	model[3][1] = position.y;

	fragment_color = vertex_color;
	fragment_texcoords = vertex_texcoords;
	gl_Position = window * model * vec4(vertex_position, 0.0, 1.0);
}
";
		public static string FragmentShaderSource = @"
#version 130

in vec4 fragment_color;
in vec2 fragment_texcoords;

out vec4 final_color;

void main()
{
	//final_color = vec4(fragment_texcoords, 1.0, 1.0);
	final_color = fragment_color;
}
";
	}
}
