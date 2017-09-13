using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoImg.Core.Common
{
    public class AutoImgCfg
    {

        /// <summary>
        /// 真实图片目录
        /// </summary>
        public string BaseDir { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<SizeType> Types
        {
            get; set;
        }
    }
}
