using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace AutoImg.Fx.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class VImgFile : VirtualFile
    {
        /// <summary>
        /// 规则
        /// </summary>
        private static readonly Regex Reg = new Regex(@"(?<p>[\w\W]+?)\.auto\.(?<type>[\w\d]+)\.(?<ext>gif|png|jpg)$", RegexOptions.IgnoreCase);

        /// <summary>
        /// 目标文件夹
        /// </summary>
        private string BaseDir { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="baseDir"></param>
        public VImgFile(string virtualPath, string baseDir) : base(virtualPath)
        {
            this.BaseDir = baseDir;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Stream Open()
        {
            //目标文件路径
            var path = Path.Combine(this.BaseDir, this.VirtualPath.TrimStart('/'));

            if (!File.Exists(path))
            {
                var ma = Reg.Match(path);
                //满足规则
                if (ma.Success)
                {
                    var p = ma.Groups["p"].Value;
                    var ext = ma.Groups["ext"].Value;
                    var type = ma.Groups["type"].Value;
                    var sizeType = JsonConfig.Get<SizeTypeConfig>()?.Types?.FirstOrDefault(t => t.Name.Equals(type, StringComparison.OrdinalIgnoreCase));

                    //规则拆分后的源原文件
                    var orgPath = $"{p}.{ext}";

                    //如果源文件存在, 如果规则存在
                    if (sizeType != null && File.Exists(orgPath))
                    {
                        if (sizeType != null)
                        {
                            ImageResizer.Deal(orgPath, sizeType, path);
                        }
                    }
                }
            }
            //如果目标文件已存在,直接返回
            return File.Open(path, FileMode.Open, FileAccess.Read);
        }





        public static bool IsMatch(string virtualPath, string baseDir, string siteRoot)
        {
            var path = Path.Combine(baseDir, virtualPath.TrimStart('/'));
            if (File.Exists(path))
            {
                return !baseDir.Equals(siteRoot, StringComparison.OrdinalIgnoreCase);
            }
            else if (Reg.IsMatch(virtualPath))
            {
                var ma = Reg.Match(path);
                var p = ma.Groups["p"].Value;
                var ext = ma.Groups["ext"].Value;
                var orgPath = $"{p}.{ext}";
                if (File.Exists(orgPath))
                {
                    var type = ma.Groups["type"].Value;
                    return JsonConfig.Get<SizeTypeConfig>()?.Types?.Any(t => t.Name.Equals(type, StringComparison.OrdinalIgnoreCase)) == true;
                }
            }

            return false;
        }

    }
}