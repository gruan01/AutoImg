using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using AutoImg.Core.Common;

namespace AutoImg.Core
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            JsonConfig.Regist<SizeTypeConfig>();
            
            var ap = new AutoImgFileProvider(@"d:\Imgs");
            var cp = new CompositeFileProvider(env.WebRootFileProvider, ap);
            var opt = new StaticFileOptions()
            {
                FileProvider = cp
            };

            app.UseStaticFiles(opt);
        }
    }
}
