using AutoImg.Core.Common;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoImg.Core
{
    public class AutoImgFileProvider : IFileProvider, IDisposable
    {

        private static readonly Regex Reg = new Regex(@"(?<p>[\w\W]+?)\.auto\.(?<type>[\w\d]+)\.(?<ext>gif|png|jpg)$", RegexOptions.IgnoreCase);

        private PhysicalFileProvider Phyical;

        private AutoImgCfg Cfg;

        private IServiceProvider SP;

        public AutoImgFileProvider(AutoImgCfg cfg, IServiceProvider sp)
        {
            this.SP = sp;
            this.Cfg = cfg;
            this.Phyical = new PhysicalFileProvider(cfg.BaseDir);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return this.Phyical.GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {

            var a = this.SP.GetService<IOptions<AutoImgCfg>>();

            var fi = this.Phyical.GetFileInfo(subpath);
            if (!fi.Exists)
            {
                var ma = Reg.Match(subpath.TrimStart('/'));
                if (ma.Success)
                {

                    var path = Path.Combine(this.Phyical.Root, subpath.TrimStart('/'));

                    var p = ma.Groups["p"].Value;
                    var ext = ma.Groups["ext"].Value;
                    var orgPath = Path.Combine(this.Phyical.Root, $"{p}.{ext}");
                    if (File.Exists(orgPath))
                    {
                        var type = ma.Groups["type"].Value;
                        var sizeType = this.Cfg?.Types?.FirstOrDefault(t => t.Name.Equals(type, StringComparison.OrdinalIgnoreCase));
                        if (sizeType != null)
                        {
                            ImageResizer.Deal(orgPath, sizeType, path);
                            fi = this.Phyical.GetFileInfo(subpath);
                        }
                    }
                }
            }
            return fi;
        }

        public IChangeToken Watch(string filter)
        {
            return this.Phyical.Watch(filter);
        }


        #region dispose
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        ~AutoImgFileProvider()
        {
            this.Dispose(false);
        }


        private bool isDisposed = false;
        private void Dispose(bool flag)
        {
            if (!isDisposed)
            {
                if (flag)
                {
                    if (this.Phyical != null)
                    {
                        this.Phyical.Dispose();
                        this.Phyical = null;
                    }
                }
                isDisposed = true;
            }
        }
        #endregion
    }
}
