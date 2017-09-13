using ImageProcessor;
using ImageProcessor.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AutoImg.Fx.Common
{
    public class ImageResizer
    {

        public static void Deal(string path, SizeType st, string destPath)
        {
            var size = new Size(st.Width, st.Height);
            using (var img = new ImageFactory())
            {
                img.Load(path)
                    .Resize(new ResizeLayer(size, st.Model ?? ResizeMode.Crop))
                    .Quality(st.Quality ?? 100)
                    .Save(destPath);
            }
        }
    }
}
