using Firefly2.Components;
using Firefly2.Exceptions;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Facilities
{
	public class Layer
	{
		private class Location
		{
			public Chunk Chunk;
			public Parcel Parcel;

			public Location(Chunk chunk, Parcel parcel)
			{
				Chunk = chunk;
				Parcel = parcel;
			}
		}

		private List<Chunk> chunks;
		private Dictionary<RenderBufferComponent, Location> bufferMap;
		private SortedSet<short> freeIndices;
		private Dictionary<RenderBufferComponent, short> transformMap;
		private float[] uniforms;
		private int tbo;
		private int texture;
		private ShaderProgramInfo shaderInfo;
		private bool needsUpdate = false;
		private const int elementsPerMatrix = 6;

		public Layer(ShaderProgramInfo shaderInfo)
		{
			chunks = new List<Chunk>();
			bufferMap = new Dictionary<RenderBufferComponent, Location>();
			transformMap = new Dictionary<RenderBufferComponent, short>();
			freeIndices = new SortedSet<short>();
			uniforms = new float[1024 * 8 * elementsPerMatrix];
			uniforms[0] = 1;
			uniforms[3] = 1;
			for (short i = 1; i < uniforms.Length / elementsPerMatrix; ++i) freeIndices.Add(i);

			this.shaderInfo = shaderInfo;

			GL.GenBuffers(1, out tbo);
			GL.BindBuffer(BufferTarget.TextureBuffer, tbo);
			GL.BufferData<float>(
				BufferTarget.TextureBuffer,
				(IntPtr)(uniforms.Length * sizeof(float)),
				uniforms, BufferUsageHint.StreamDraw);
			texture = GL.GenTexture();
			GL.BindTexture(TextureTarget.TextureBuffer, texture);
			GL.TexBuffer(TextureBufferTarget.TextureBuffer, SizedInternalFormat.R32f, tbo);
		}

		public void ModifyOrAddRenderBuffer(RenderBufferComponent buffer)
		{
			if (bufferMap.ContainsKey(buffer))
			{
				var loc = bufferMap[buffer];
				if (!loc.Chunk.TryModify(loc.Parcel, buffer.Data))
				{
					bool added = false;
					foreach (var chunk in chunks)
					{
						if (chunk == loc.Chunk) continue;
						if (chunk.TryAdd(loc.Parcel, buffer.Data))
						{
							added = true;
							loc.Chunk = chunk;
							break;
						}
					}
					if (!added)
					{
						var chunk = new Chunk(shaderInfo);
						chunks.Add(chunk);
						chunk.TryAdd(loc.Parcel, buffer.Data);
						loc.Chunk = chunk;
					}
				}
			}
			else
			{
				var parcel = new Parcel(0, 0);
				var chunk = chunks.FirstOrDefault(c => c.TryAdd(parcel, buffer.Data));
				if (chunk == null)
				{
					chunk = new Chunk(shaderInfo);
					chunks.Add(chunk);
					chunk.TryAdd(parcel, buffer.Data);
				}
				bufferMap[buffer] = new Location(chunk, parcel);
			}
		}

		private static byte[] emptyByteArray = new byte[0];
		public void RemoveRenderBuffer(RenderBufferComponent buffer)
		{
			if (bufferMap.ContainsKey(buffer))
			{
				var loc = bufferMap[buffer];
				loc.Chunk.TryModify(loc.Parcel, emptyByteArray);
			}
		}

		public short ModifyOrAddTransform(RenderBufferComponent renderBuffer, Matrix4 matrix)
		{
			if (!transformMap.ContainsKey(renderBuffer))
			{
				if (freeIndices.Count == 0) throw new LayerFullException();
				transformMap[renderBuffer] = freeIndices.First();
				freeIndices.Remove(freeIndices.First());
			}
			int index = transformMap[renderBuffer] * elementsPerMatrix;
			int offset = 0;
			for (int i = 0; i < 2; ++i)
			{
				for (int j = 0; j < 2; ++j)
				{
					uniforms[index + offset] = matrix[i, j];
					offset++;
				}
			}
			uniforms[index + offset] = (float)matrix[3, 0];
			uniforms[index + offset + 1] = (float)matrix[3, 1];
			needsUpdate = true;

			return transformMap[renderBuffer];
		}

		public void RemoveTransform(RenderBufferComponent renderBuffer)
		{
			if (transformMap.ContainsKey(renderBuffer))
			{
				freeIndices.Add(transformMap[renderBuffer]);
				transformMap.Remove(renderBuffer);
			}
		}

		public void FinishUpdate()
		{
			if (!needsUpdate) return;

			GL.BindBuffer(BufferTarget.TextureBuffer, tbo);
			GL.BufferData<float>(
				BufferTarget.TextureBuffer,
				(IntPtr)(uniforms.Length * sizeof(float)),
				uniforms, BufferUsageHint.StreamDraw);

			needsUpdate = false;
		}

		public void Render()
		{
			GL.UseProgram(shaderInfo.ShaderProgram);
			GL.BindTexture(TextureTarget.TextureBuffer, texture);
			GL.Uniform1(shaderInfo.TexBuffer, 0);
			GL.UniformMatrix4(shaderInfo.WindowLocation, false, ref shaderInfo.Window);
			foreach (var chunk in chunks) chunk.Render();
		}
	}
}
