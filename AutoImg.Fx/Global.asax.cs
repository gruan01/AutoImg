using AutoImg.Fx.Common;
using System;
using System.Configuration;
using System.Web;
using System.Web.Hosting;

namespace AutoImg.Fx
{
    public class Global : System.Web.HttpApplication
    {

        /// <summary>
        /// https://msdn.microsoft.com/zh-cn/library/system.web.hosting.virtualpathprovider(VS.80).aspx
        /// 为进行部署而预编译网站时，将不编译 VirtualPathProvider 实例提供的内容，预编译站点不使用任何 VirtualPathProvider 实例。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {
            //要映射的真实的图片目录
            var baseDir = ConfigurationManager.AppSettings.Get("BaseDir");
            if (string.IsNullOrWhiteSpace(baseDir))
                baseDir = AppDomain.CurrentDomain.BaseDirectory;
            else
            {
                //
                Environment.CurrentDirectory = baseDir;
            }
            //注册 JsonConfig
            JsonConfig.Regist<SizeTypeConfig>();

            //注册虚拟文件提供器
            HostingEnvironment.RegisterVirtualPathProvider(new VImgProvider(baseDir));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //设置文件缓存头,如果该网站还提供其它服务,请注释这几句代码
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetExpires(DateTime.Now.AddYears(1));
            Response.Cache.SetMaxAge(TimeSpan.FromDays(365));
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}