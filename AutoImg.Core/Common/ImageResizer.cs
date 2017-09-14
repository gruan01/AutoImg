using ImageSharp;
using ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoImg.Core.Common
{
    public class ImageResizer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">原图</param>
        /// <param name="st"></param>
        /// <param name="destPath">新图保存路径</param>
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
                //ImageSharp 不支持质量百分比
                //.Quality(st.Quality ?? 100)

                //保存
                .Save(destPath);
            }
        }
    }
}
