﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportEvent.Extensions;

namespace SportEvent
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureBll();
            services.ConfigureLoggerService();
            services.ConfigureAuthentication();
            services.ConfigureCors();
            services.ConfigureJwtAuthen(Configuration);

            services.AddMvc(opt =>
            {
                opt.UseApiGlobalConfigRoutePrefix(new RouteAttribute("api"));
            });
            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.ConfigureMiddleware();
            app.ConfigureSwagger();
            app.UseCors("CorsPolicy");
            app.UseMvc();
        }
    }
}
