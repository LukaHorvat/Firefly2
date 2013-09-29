using Firefly2.Components;
using System;
using System.Collections.Generic;
using System.Linq;
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

		private class Parcel
		{
			public int Index;
			public int Size;

			public Parcel(int index, int size)
			{
				Index = index;
				Size = size;
			}
		}

		private class Chunk
		{
			private byte[] data;
			private List<Parcel> parcels;
			private int filledSize;

			public Chunk()
			{
				data = new byte[((2 * 4 + 4 + 2 + 2) * 3) * 5000];
				parcels = new List<Parcel>();
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
				filledSize += newData.Length;
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
						copyTo += parcels[i].Size;
					}
				}
				filledSize = copyTo;
			}
		}

		private List<Chunk> chunks;
		private Dictionary<RenderBufferComponent, Location> bufferMap;

		public Layer()
		{
			chunks = new List<Chunk>();
			bufferMap = new Dictionary<RenderBufferComponent, Location>();
		}

		public void ModifyOrAdd(RenderBufferComponent buffer)
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
						var chunk = new Chunk();
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
					chunk = new Chunk();
					chunks.Add(chunk);
					chunk.TryAdd(parcel, buffer.Data);
				}
				bufferMap[buffer] = new Location(chunk, parcel);
			}
		}
	}
}
