using Firefly2.Components;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Facilities
{
	public class Renderer
	{
		private Dictionary<string, Layer> layers;
		private int window;
		private ShaderProgramInfo shaderInfo;

		public int ShaderProgram;
		public Matrix4 WindowMatrix;

		public Renderer(int width, int height)
		{
			//TODO: Implement custom shaders and cross-renderer layers
			layers = new Dictionary<string, Layer>();

			string debug;

			int vertexShader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, DefaultShaders.VertexShaderSource);
			GL.CompileShader(vertexShader);
			GL.GetShaderInfoLog(vertexShader, out debug);
			Console.WriteLine(debug);

			int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, DefaultShaders.FragmentShaderSource);
			GL.CompileShader(fragmentShader);
			GL.GetShaderInfoLog(fragmentShader, out debug);
			Console.WriteLine(debug);

			ShaderProgram = GL.CreateProgram();
			GL.AttachShader(ShaderProgram, vertexShader);
			GL.AttachShader(ShaderProgram, fragmentShader);
			GL.LinkProgram(ShaderProgram);

			window = GL.GetUniformLocation(ShaderProgram, "window");
			WindowMatrix = Matrix4.CreateScale(2F / width, 2F / height, 1);

			shaderInfo = new ShaderProgramInfo
			{
				ShaderProgram = ShaderProgram,
				Window = WindowMatrix,
				VertexPosition = GL.GetAttribLocation(ShaderProgram, "vertex_position"),
				VertexColor = GL.GetAttribLocation(ShaderProgram, "vertex_color"),
				VertexTexCoords = GL.GetAttribLocation(ShaderProgram, "vertex_texcoords"),
				VertexIndex = GL.GetAttribLocation(ShaderProgram, "index"),
				TexBuffer = GL.GetUniformLocation(ShaderProgram, "buffer"),
				Atlas = GL.GetUniformLocation(ShaderProgram, "atlas"),
				WindowLocation = window
			};

			GL.BindFragDataLocation(ShaderProgram, 0, "fragment_color");
		}

		public Layer GetLayer(string name)
		{
			Layer layer;
			if (!layers.TryGetValue(name, out layer))
			{
				layer = new Layer(shaderInfo);
				layers[name] = layer;
			}
			return layer;
		}

		public void ProcessRenderBuffer(RenderBufferComponent buffer)
		{
			GetLayer("default").ModifyOrAddRenderBuffer(buffer);
		}

		public void RemoveRenderBuffer(RenderBufferComponent buffer)
		{
			GetLayer("default").RemoveRenderBuffer(buffer);
		}

		public short ProcessTransform(RenderBufferComponent renderBuffer, Matrix4 matrix)
		{
			return GetLayer("default").ModifyOrAddTransform(renderBuffer, matrix);
		}

		public void RemoveTransformAndTexture(RenderBufferComponent renderBuffer)
		{
			GetLayer("default").RemoveTransformAndTexture(renderBuffer);
		}

		public short ProcessTexture(RenderBufferComponent renderBuffer, Bitmap bmp)
		{
			return GetLayer("default").ModifyOrAddTexture(renderBuffer, bmp);
		}

		public void Render()
		{
			foreach (var layer in layers.Values)
			{
				layer.Render();
			}
		}

		public void FinishUpdate()
		{
			foreach (var layer in layers.Values)
			{
				layer.FinishUpdate();
			}
		}
	}
}
