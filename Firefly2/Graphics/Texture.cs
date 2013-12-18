using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly2.Graphics
{
	public class Texture
	{
		private Bitmap bitmap;

		public Texture(string path)
			: this(new Bitmap(path)) { }

		public Texture(Bitmap bmp)
		{
			UpdateBitmap(bmp);
		}

		public Texture()
		{

		}

		public void UpdateBitmap(Bitmap bmp)
		{
			bitmap = bmp;
			RegenerateTexture();
		}

		private void RegenerateTexture()
		{

		}
	}
}
