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
    public class VImgFile : VirtualFile
    {
        private static readonly string[] ImgExts = new string[] { "GIF", "JPG", "PNG" };
        private static readonly Regex Reg = new Regex(@"(?<p>[\w\W]+?)\.auto\.(?<type>[\w\d]+)\.(?<ext>gif|png|jpg)$", RegexOptions.IgnoreCase);

        private static readonly string SiteRoot = AppDomain.CurrentDomain.BaseDirectory;

        private string BaseDir { get; }

        public VImgFile(string virtualPath, string baseDir) : base(virtualPath)
        {
            this.BaseDir = baseDir;
        }

        public override Stream Open()
        {
            var path = Path.Combine(this.BaseDir, this.VirtualPath.TrimStart('/'));

            if (!File.Exists(path))
            {
                var ma = Reg.Match(path);
                var p = ma.Groups["p"].Value;
                var ext = ma.Groups["ext"].Value;
                var orgPath = $"{p}.{ext}";
                if (File.Exists(orgPath))
                {
                    var type = ma.Groups["type"].Value;
                    var sizeType = JsonConfig.Get<SizeTypeConfig>()?.Types?.FirstOrDefault(t => t.Name.Equals(type, StringComparison.OrdinalIgnoreCase));
                    if (sizeType != null)
                    {
                        ImageResizer.Deal(orgPath, sizeType, path);
                    }
                }
            }
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