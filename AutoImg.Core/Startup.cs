using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace AutoImg.Core
{
    public class Startup
    {

        public IConfigurationRoot Configuration { get; }


        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                //加载自定义 json 配置文件
                .AddJsonFile("AutoImg.json", true, true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddOptions();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region static file
            ///
            var ap = new AutoImgFileProvider(this.Configuration);
            //优先 WebRootFileProvider , 如果文件在它中能找得到,就不在去 AutoImageFileProvider 中在找一次了.
            var cp = new CompositeFileProvider(env.WebRootFileProvider, ap);
            var opt = new StaticFileOptions()
            {
                FileProvider = cp
            };

            app.UseStaticFiles(opt);
            #endregion
        }
    }
}
