using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace AutoImg.Fx.Common
{

    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class VImgProvider : VirtualPathProvider
    {
        private string BaseDir;

        private bool InSiteRoot;

        public VImgProvider(string baseDir)
        {
            if (string.IsNullOrWhiteSpace(baseDir))
                throw new ArgumentNullException("baseDir");

            if (!Directory.Exists(baseDir))
                throw new ArgumentException($"baseDir: {baseDir} 不存在");

            this.BaseDir = new DirectoryInfo(baseDir).FullName;
            this.InSiteRoot = baseDir.Equals(AppDomain.CurrentDomain.BaseDirectory, StringComparison.OrdinalIgnoreCase);
        }

        public override bool FileExists(string virtualPath)
        {
            return this.GetFile(virtualPath) != null;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (VImgFile.IsMatch(virtualPath, this.BaseDir, AppDomain.CurrentDomain.BaseDirectory))
                return new VImgFile(virtualPath, this.BaseDir);
            else
                return Previous.GetFile(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return new CacheDependency("~/cachedependency.txt");
        }
    }
}