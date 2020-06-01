using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VLCTest.VLC
{
	public class BitmapSourceGenerator
	{
		public PixelFormat Format { get; private set; } = PixelFormats.Bgra32;
		public byte[] RawImage { get => rawImage; }
		public int RawStride { get => rawStride; }
		public int Width { get => width; }
		public int Height { get => height; }


		public void Init(int width, int height)
		{
			if (rawImage != null && this.Width == width && this.Height == height)
			{
				Clear();
				return;
			}

			this.width = width;
			this.height = height;
			rawStride = (width * Format.BitsPerPixel + 7) / 8;
			rawImage = new byte[rawStride * height];
		}

		public void Init(int width, int height, PixelFormat pf)
		{
			Format = pf;
			Init(width, height);
		}

		public BitmapSource RenderBitmapSource()
		{
			if (rawImage == null)
				return null;

			return BitmapSource.Create(Width, Height,
				96, 96, Format, null, rawImage, rawStride);
		}

		public void Clear()
		{
			if (RawImage == null)
				return;

			for (int i = 0; i < RawImage.Length; i++)
				RawImage[i] = 0;
		}

		int width;
		int height;
		int rawStride;
		byte[] rawImage;
	}
}
