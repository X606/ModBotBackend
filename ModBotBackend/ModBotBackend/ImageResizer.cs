using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBotBackend
{
    public static class ImageResizer
    {
        public static bool TryScaleImageAndGetAsByteArray(string id, string size, string path, ConcurrentDictionary<string, byte[]> rescaledImageCache, out byte[] data)
        {
            string[] split = size.Split('x');

            if (split.Length != 2)
            {
                data = null;
                return false;
            }

			int[] dimentions = new int[split.Length];
			for (int i = 0; i < dimentions.Length; i++)
			{
				if (int.TryParse(split[i], out int result))
				{
					dimentions[i] = result;
				}
				else
				{
					data = null;
					return false;
				}
			}

			if (Math.Max(dimentions[0], dimentions[1]) > 1024 || Math.Min(dimentions[0], dimentions[1]) < 1)
			{
				data = null;
				return false;
			}

			byte[] imageData;
			if (rescaledImageCache.TryGetValue(id + size, out byte[] imgData))
			{
				imageData = imgData;
			}
			else
			{
				using (var ms = new FileStream(path, FileMode.Open))
				{
					using (Bitmap bmp = new Bitmap(ms))
					{

						float ratio = ((float)dimentions[0]) / ((float)dimentions[1]);

						int width;
						int height;

						width = (int)(bmp.Height * ratio);
						height = bmp.Height;

						if (width > bmp.Width)
						{
							ratio = ((float)bmp.Width) / ((float)width);
							width = bmp.Width;
							height = (int)(bmp.Width * ratio);
						}

						using (Bitmap cropped = bmp.CropImageAtRect(new Rectangle((bmp.Width - width) / 2, (bmp.Height - height) / 2, width, height)))
						{
							using (Bitmap resized = Utils.ResizeImage(cropped, dimentions[0], dimentions[1], System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic))
							{

								using (var saveStream = new MemoryStream())
								{
									resized.Save(saveStream, ImageFormat.Png);

									imageData = saveStream.ToArray();
								}
							}
						}
					}
				}
				rescaledImageCache.TryAdd(id + size, imageData);
			}

			data = imageData;
			return true;
        }
    }
}
