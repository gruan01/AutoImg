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
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace AutoImg.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoImgFileProvider : IFileProvider, IDisposable
    {

        private static readonly Regex Reg = new Regex(@"(?<p>[\w\W]+?)\.auto\.(?<type>[\w\d]+)\.(?<ext>gif|png|jpg)$", RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        private PhysicalFileProvider Phyical;

        /// <summary>
        /// 
        /// </summary>
        private AutoImgCfg Cfg;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public AutoImgFileProvider(IConfigurationRoot configuration)
        {
            //必须先实例, 如果是 null, bind 之后还是 null
            this.Cfg = new AutoImgCfg();
            configuration.Bind("AutoImg", this.Cfg);

            //如果未设置,或目标文件夹不存,则使用当前目录
            if (!string.IsNullOrWhiteSpace(this.Cfg.BaseDir) && Directory.Exists(this.Cfg.BaseDir))
                this.Phyical = new PhysicalFileProvider(this.Cfg.BaseDir);
            else
                this.Phyical = new PhysicalFileProvider(AppContext.BaseDirectory);

            //监听配置变化,以更新配置
            //因为不受DI影响,所以这里如果使用 IOptions / IOptionsSnapshot
            //是不会得到最新配置的
            ChangeToken.OnChange(
                () => configuration.GetReloadToken(),
                () =>
                {
                    //不能直 bind 到 Cfg 上, 并不会更新 Cfg
                    var tmp = new AutoImgCfg();
                    configuration.Bind("AutoImg", tmp);
                    this.Cfg = tmp;

                    //如果未设置,或目标文件夹不存,则不更改
                    if (!string.Equals(tmp.BaseDir, this.Phyical.Root) && Directory.Exists(this.Cfg.BaseDir))
                        this.Phyical = new PhysicalFileProvider(this.Cfg.BaseDir ?? AppContext.BaseDirectory);
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subpath"></param>
        /// <returns></returns>
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return this.Phyical.GetDirectoryContents(subpath);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="subpath"></param>
        /// <returns></returns>
        public IFileInfo GetFileInfo(string subpath)
        {
            var fi = this.Phyical.GetFileInfo(subpath);
            if (!fi.Exists)
            {
                var ma = Reg.Match(subpath.TrimStart('/'));
                //如果目标不存在,且满足规则
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
                            //处理原文件,并保存
                            ImageResizer.Deal(orgPath, sizeType, path);
                            //重新获取新文件信息
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
