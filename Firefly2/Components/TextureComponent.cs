﻿using Firefly2.Messages;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Components
{
	public class TextureComponent : Component
	{
		public ObservableCollection<Vector2> TexCoords;

		public TextureComponent()
		{
			TexCoords = new ObservableCollection<Vector2>();
			TexCoords.CollectionChanged += delegate
			{
				Host.SendMessage(TexCoordsChanged.Instance);
			};
		}

		public void UpdateBitmap(Bitmap bmp)
		{
			Host.SendMessage(new TextureChanged(bmp));
		}
	}
}
