using Firefly2.Algorithm;
using Firefly2.Components;
using Firefly2.Exceptions;
using Firefly2.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TexLib;

namespace Firefly2.Facilities
{
	public class Layer
	{
		public const int ElementsPerObject = 11;

		private static byte[] emptyByteArray = new byte[0];

		private Dictionary<RenderBufferComponent, Location> bufferMap;

		private List<Chunk> chunks;

		private SortedSet<short> freeIndices;

		private bool needsUpdate = false;

		private ShaderProgramInfo shaderInfo;

		private int textureBufferObject;

		private int textureBufferTexture;

		private int textureAtlasTexture;

		private Dictionary<RenderBufferComponent, short> transformMap;

		private Dictionary<short, Tuple<PackedRectangle, Bitmap>> atlasParcels;

		private float[] uniforms;

		private Bitmap atlas;

		public Layer(ShaderProgramInfo shaderInfo)
		{
			chunks = new List<Chunk>();
			bufferMap = new Dictionary<RenderBufferComponent, Location>();
			transformMap = new Dictionary<RenderBufferComponent, short>();
			atlas = new Bitmap(1, 1);
			atlasParcels = new Dictionary<short, Tuple<PackedRectangle, Bitmap>>();
			freeIndices = new SortedSet<short>();
			uniforms = new float[1024 * 8 * ElementsPerObject];
			uniforms[0] = 1;
			uniforms[3] = 1;
			for (short i = 1; i < uniforms.Length / ElementsPerObject; ++i) freeIndices.Add(i);

			this.shaderInfo = shaderInfo;

			GL.GenBuffers(1, out textureBufferObject);
			GL.BindBuffer(BufferTarget.TextureBuffer, textureBufferObject);
			GL.BufferData<float>(
				BufferTarget.TextureBuffer,
				(IntPtr)(uniforms.Length * sizeof(float)),
				uniforms, BufferUsageHint.StreamDraw);
			textureBufferTexture = GL.GenTexture();
			GL.BindTexture(TextureTarget.TextureBuffer, textureBufferTexture);
			GL.TexBuffer(TextureBufferTarget.TextureBuffer, SizedInternalFormat.R32f, textureBufferObject);

			textureAtlasTexture = GL.GenTexture();
		}

		public void FinishUpdate()
		{
			if (!needsUpdate) return;

			GL.BindBuffer(BufferTarget.TextureBuffer, textureBufferObject);
			GL.BufferData<float>(
				BufferTarget.TextureBuffer,
				(IntPtr)(uniforms.Length * sizeof(float)),
				uniforms, BufferUsageHint.StreamDraw);

			needsUpdate = false;
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

		public short ModifyOrAddTransform(RenderBufferComponent renderBuffer, Matrix4 matrix)
		{
			int index = GetOrAllocateIndex(renderBuffer) * ElementsPerObject;
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
			uniforms[index + offset + 2] = (float)matrix[3, 2];

			//Add -1 as texture coordinates so that objects without textures can be recognized
			if (!atlasParcels.ContainsKey(GetOrAllocateIndex(renderBuffer)))
			{
				uniforms[index + offset + 3] = -1;
				uniforms[index + offset + 4] = -1;
				uniforms[index + offset + 5] = -1;
				uniforms[index + offset + 6] = -1;
			}
			needsUpdate = true;

			return transformMap[renderBuffer];
		}

		public short ModifyOrAddTexture(RenderBufferComponent renderBuffer, Bitmap bmp)
		{
			short index = GetOrAllocateIndex(renderBuffer);
			Tuple<PackedRectangle, Bitmap> parcel = null;
			atlasParcels.TryGetValue(index, out parcel);
			if (parcel == null || parcel.Item1.Rectangle.Size.X != bmp.Width || parcel.Item1.Rectangle.Size.Y != bmp.Width)
			{
				Console.WriteLine("New bitmap is different in size");
				parcel = new Tuple<PackedRectangle, Bitmap>
				(
					new PackedRectangle
					(
						new AARectangle
						(
							Vector2d.Zero,
							new Vector2d(bmp.Width, bmp.Height)
						),
						index //The id will be the link between the rectangle and the parcel
					),
					bmp
				);
				atlasParcels[index] = parcel;
				MakeAtlas();
				ResetTextureBounds();
			}
			else
			{
				var gfx = Graphics.FromImage(atlas);
				gfx.DrawImage(bmp, parcel.Item1.Rectangle.Position.ToDrawingPoint());
				gfx.Dispose();
			}
			return index;
		}

		private void MakeAtlas()
		{
			var rectangles = atlasParcels.Values.Select(t => t.Item1).ToList();
			RectanglePacking.PackRectanglesModifyFSharp(rectangles);
			var atlasSize = (int)Math.Max
			(
				rectangles.Max(rect => rect.Rectangle.Position.X + rect.Rectangle.Size.X),
				rectangles.Max(rect => rect.Rectangle.Position.Y + rect.Rectangle.Size.Y)
			);
			if (atlas.Width != atlasSize)
			{
				atlas.Dispose();
				atlas = new Bitmap(atlasSize, atlasSize);
			}
			var gfx = Graphics.FromImage(atlas);
			foreach (var rect in rectangles)
			{
				var bmp = atlasParcels[(short)rect.Id].Item2;
				gfx.DrawImage(bmp, new Rectangle(rect.Rectangle.Position.ToDrawingPoint(), bmp.Size));
			}
			gfx.Dispose();

			var data = atlas.LockBits(
			  new Rectangle(0, 0, atlas.Width, atlas.Height),
			  System.Drawing.Imaging.ImageLockMode.ReadOnly,
			  System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			GL.BindTexture(TextureTarget.Texture2D, textureAtlasTexture);
			GL.TexImage2D(
			  TextureTarget.Texture2D,
			  0,
			  PixelInternalFormat.Rgba,
			  data.Width, data.Height,
			  0,
			  PixelFormat.Bgra,
			  PixelType.UnsignedByte,
			  data.Scan0);
			atlas.UnlockBits(data);
			TexUtil.SetParameters();
		}

		/// <summary>
		/// Calculates the point where each texture starts and ends in tex coords and stores it in the uniform array
		/// </summary>
		private void ResetTextureBounds()
		{
			float factor = 1F / atlas.Width;
			foreach (var parcel in atlasParcels)
			{
				var start = parcel.Value.Item1.Rectangle.Position;
				var end = parcel.Value.Item1.Rectangle.Size + start;
				uniforms[parcel.Key * ElementsPerObject + 7] = (float)start.X * factor;
				uniforms[parcel.Key * ElementsPerObject + 8] = (float)start.Y * factor;
				uniforms[parcel.Key * ElementsPerObject + 9] = (float)end.X * factor;
				uniforms[parcel.Key * ElementsPerObject + 10] = (float)end.Y * factor;
			}
		}

		private short GetOrAllocateIndex(RenderBufferComponent renderBuffer)
		{
			if (!transformMap.ContainsKey(renderBuffer))
			{
				if (freeIndices.Count == 0) throw new LayerFullException();
				transformMap[renderBuffer] = freeIndices.First();
				freeIndices.Remove(freeIndices.First());
			}
			return transformMap[renderBuffer];
		}

		public void RemoveRenderBuffer(RenderBufferComponent buffer)
		{
			if (bufferMap.ContainsKey(buffer))
			{
				var loc = bufferMap[buffer];
				loc.Chunk.TryModify(loc.Parcel, emptyByteArray);
			}
		}

		public void RemoveTransformAndTexture(RenderBufferComponent renderBuffer)
		{
			if (transformMap.ContainsKey(renderBuffer))
			{
				var index = transformMap[renderBuffer];
				if (atlasParcels.ContainsKey(index))
				{
					atlasParcels.Remove(index);
				}
				freeIndices.Add(index);
				transformMap.Remove(renderBuffer);
			}
		}

		public void Render()
		{
			GL.UseProgram(shaderInfo.ShaderProgram);

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.TextureBuffer, textureBufferTexture);
			GL.Uniform1(shaderInfo.TexBuffer, 0);

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, textureAtlasTexture);
			GL.Uniform1(shaderInfo.Atlas, 1);

			GL.UniformMatrix4(shaderInfo.WindowLocation, false, ref shaderInfo.Window);
			foreach (var chunk in chunks) chunk.Render();
		}

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
	}
}