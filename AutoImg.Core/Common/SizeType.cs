using ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoImg.Core.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class SizeType
    {
        /// <summary>
        /// 
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 裁剪模式
        /// </summary>
        public ResizeMode? Model { get; set; }

        /// <summary>
        /// 质量百分比(0~100)
        /// </summary>
        public int? Quality { get; set; }
    }
}
