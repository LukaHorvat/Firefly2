using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Facilities
{
	internal class Parcel
	{
		public int Index;
		public int Size;

		public Parcel(int index, int size)
		{
			Index = index;
			Size = size;
		}
	}

	internal class Chunk
	{
		private byte[] data;
		private List<Parcel> parcels;
		private int filledSize;
		private int vbo;
		private ShaderProgramInfo shaderInfo;

		public Chunk(ShaderProgramInfo shaderInfo)
		{
			data = new byte[((2 * 4 + 4 + 2 + 2) * 3) * 5000];
			parcels = new List<Parcel>();

			this.shaderInfo = shaderInfo;

			GL.GenBuffers(1, out vbo);
		}

		/// <summary>
		/// Returns true if the new data was successfully refited in this chunk. 
		/// Otherwise it just removes the previous version and returns false.
		/// </summary>
		/// <param name="parcel"></param>
		/// <param name="newData"></param>
		/// <returns></returns>
		public bool TryModify(Parcel parcel, byte[] newData)
		{
			int index = parcels.IndexOf(parcel);
			if (index != -1)
			{
				if (parcel.Size != newData.Length)
				{
					parcels.RemoveAt(index);
					Refit();
				}
				else
				{
					Buffer.BlockCopy(newData, 0, data, parcel.Index, newData.Length);
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
					GL.BufferSubData<byte>(BufferTarget.ArrayBuffer, (IntPtr)parcel.Index, (IntPtr)parcel.Size, newData);
					return true;
				}
			}
			return TryAdd(parcel, newData);
		}

		/// <summary>
		/// Add new data to the chunk. Returns true if there's room, otherwise false.
		/// </summary>
		/// <param name="oldParcel">Parcel that will contain new data if there was room</param>
		/// <param name="newData"></param>
		/// <returns></returns>
		public bool TryAdd(Parcel oldParcel, byte[] newData)
		{
			if (filledSize + newData.Length >= data.Length) return false;
			Buffer.BlockCopy(newData, 0, data, filledSize, newData.Length);
			oldParcel.Index = filledSize;
			oldParcel.Size = newData.Length;
			parcels.Add(oldParcel);
			filledSize += newData.Length;

			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData<byte>(BufferTarget.ArrayBuffer, (IntPtr)filledSize, data, BufferUsageHint.DynamicDraw);
			return true;
		}

		private void Refit()
		{
			int copyTo = 0;
			for (int i = 0; i < parcels.Count; ++i)
			{
				if (parcels[i].Index != copyTo)
				{
					Buffer.BlockCopy(data, parcels[i].Index, data, copyTo, parcels[i].Size);
					parcels[i].Index = copyTo;
				}
				copyTo += parcels[i].Size;
			}
			filledSize = copyTo;
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData<byte>(BufferTarget.ArrayBuffer, (IntPtr)filledSize, data, BufferUsageHint.DynamicDraw);
		}

		public void Render()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

			GL.EnableVertexAttribArray(shaderInfo.VertexPosition);
			GL.EnableVertexAttribArray(shaderInfo.VertexColor);
			GL.EnableVertexAttribArray(shaderInfo.VertexTexCoords);
			GL.EnableVertexAttribArray(shaderInfo.VertexIndex);

			GL.VertexAttribPointer(shaderInfo.VertexPosition, 2, VertexAttribPointerType.Float,
				false, 16, 0);
			GL.VertexAttribPointer(shaderInfo.VertexColor, 4, VertexAttribPointerType.UnsignedByte,
				true, 16, 8);
			GL.VertexAttribPointer(shaderInfo.VertexTexCoords, 2, VertexAttribPointerType.UnsignedByte,
				true, 16, 12);
			GL.VertexAttribPointer(shaderInfo.VertexIndex, 1, VertexAttribPointerType.Short,
				true, 16, 14);
			GL.DrawArrays(BeginMode.Triangles, 0, filledSize / 16);

			GL.DisableVertexAttribArray(shaderInfo.VertexPosition);
			GL.DisableVertexAttribArray(shaderInfo.VertexColor);
			GL.DisableVertexAttribArray(shaderInfo.VertexTexCoords);
			GL.DisableVertexAttribArray(shaderInfo.VertexIndex);

		}
	}
}
