using Firefly2.Components;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Facilities
{
	public class Renderer
	{
		private Dictionary<string, Layer> layers;
		private int window;
		private Matrix4 windowMatrix;
		private ShaderProgramInfo shaderInfo;

		public int ShaderProgram;

		public Renderer(int width, int height)
		{
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
			windowMatrix = Matrix4.CreateScale(2F / width, -2F / height, 1);

			shaderInfo = new ShaderProgramInfo
			{
				ShaderProgram = ShaderProgram,
				Window = windowMatrix,
				VertexPosition = GL.GetAttribLocation(ShaderProgram, "vertex_position"),
				VertexColor = GL.GetAttribLocation(ShaderProgram, "vertex_color"),
				VertexTexCoords = GL.GetAttribLocation(ShaderProgram, "vertex_texcoords"),
				VertexIndex = GL.GetAttribLocation(ShaderProgram, "index"),
				TexBuffer = GL.GetUniformLocation(ShaderProgram, "buffer"),
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
			GetLayer("default").ModifyOrAdd(buffer);
		}

		public void RemoveRenderBuffer(RenderBufferComponent buffer)
		{
			GetLayer("default").Remove(buffer);
		}

		public short ProcessTransform(TransformComponent transform)
		{
			return GetLayer("default").ModifyOrAdd(transform);
		}

		public void RemoveTransform(TransformComponent transform)
		{
			GetLayer("default").Remove(transform);
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
