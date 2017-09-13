using ImageSharp;
using ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoImg.Core.Common
{
    public class ImageResizer
    {

        public static void Deal(string path, SizeType st, string destPath)
        {
            var size = new Size(st.Width, st.Height);
            using (var img = new Image(path))
            {
                img.Resize(new ResizeOptions()
                {
                    Mode = st.Model ?? ResizeMode.Crop,
                    Size = size
                })
                .Quantize(Quantization.Octree)
                //.Quality(st.Quality ?? 100)
                .Save(destPath);
            }
        }
    }
}
