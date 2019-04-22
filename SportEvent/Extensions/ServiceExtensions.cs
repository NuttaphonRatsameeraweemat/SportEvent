using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using SportEvent.Data;
using SportEvent.Data.Repository.Interfaces;
using SportEvent.Helper;
using SportEvent.Helper.Interfaces;
using SportEvent.Bll.Interfaces;
using SportEvent.Bll;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace SportEvent.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Dependency Injection Repository and UnitOfWork.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="Configuration">The configuration from settinfile.</param>
        public static void ConfigureRepository(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddEntityFrameworkSqlServer()
             .AddDbContext<DbContext>(options =>
              options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));

            services.AddTransient<IUnitOfWork, SeUnitOfWork>();
        }

        /// <summary>
        /// Dependency Injection Class Business Logic Layer.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void ConfigureBll(this IServiceCollection services)
        {
            services.AddScoped<IEvent, Event>();
            services.AddScoped<ILogin, LoginBll>();
        }

        /// <summary>
        /// Add Singletion Logger Class.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        /// <summary>
        /// Add CORS Configuration.
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }
        
        /// <summary>
        /// Config Api Routes Prefix.
        /// </summary>
        /// <param name="opts">The MvcOptions.</param>
        /// <param name="routeAttribute">The IRouteTemplateProvider.</param>
        public static void UseApiGlobalConfigRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            opts.Conventions.Insert(0, new ApiGlobalPrefixRouteProvider(routeAttribute));
        }

        /// <summary>
        /// Add Swagger.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

                // Swagger 2.+ support
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "Header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);

            });
        }

        /// <summary>
        /// Add Swagger.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void ConfigureSwagger(this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        /// <summary>
        /// Add Middleware when request bein and end.
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<Middleware>();
        }

        /// <summary>
        /// Configure Basic Authentication
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void ConfigureAuthentication(this IServiceCollection services)
        {
            services.AddScoped<BasicAuthAttribute>();
        }

        /// <summary>
        /// Add Jwt Authentication and Setting.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        public static void ConfigureJwtAuthen(this IServiceCollection services, IConfiguration Configuration)
        {
            var option = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = System.TimeSpan.Zero,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
            };
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = option;
                 options.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                         var model = new
                         {
                             context.Response.StatusCode,
                             Message = "Unauthorized."
                         };
                         string json = JsonConvert.SerializeObject(model, new JsonSerializerSettings
                         {
                             ContractResolver = new CamelCasePropertyNamesContractResolver()
                         });
                         context.Response.OnStarting(async () =>
                         {
                             context.Response.ContentType = "application/json";
                             await context.Response.WriteAsync(json);
                         });
                         return System.Threading.Tasks.Task.CompletedTask;
                     },
                 };
             });
        }

    }
}
