using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.AspNetCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Reflection;
using Boilerplate.Web.Context;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Web
{
    /// <summary>
    /// Startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Startup method
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region EntityFramework
            services.AddDbContext<BoilerplateContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            #endregion

            #region MVC & Razor pages
            services.AddControllersWithViews(options => {
                // Global Antiforgery Options
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            .AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));
            services.AddEndpointsApiExplorer();            
            services.AddRazorPages()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.Converters.Add(new StringEnumConverter()));
            #endregion

            #region Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Boilerplate API",
                    Description = "Boilerplate API 온라인 문서입니다.",
                    TermsOfService = new Uri("https://www.example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "연락정보",
                        Url = new Uri("https://www.example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "라이선스",
                        Url = new Uri("https://www.example.com/license")
                    }
                });

                #region Swagger filter
                options.ExampleFilters();

                // if you're using the SecurityRequirementsOperationFilter, you also need to tell Swashbuckle you're using OAuth2
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                #endregion

                // 주석 연동이 되도록 세팅
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            services.AddSwaggerGenNewtonsoftSupport(); // explicit opt-in - needs to be placed after AddSwaggerGen()
            services.AddSwaggerExamples();
            //services.AddMvcCore()
            //    .AddApiExplorer();            
            #endregion
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options => {
                    // Swagger에서 사용하는 CSS 추가
                    //options.InjectStylesheet("/swagger-ui/custom.css");
                });

                // todo : Swagger에서 Antiforgery 사용가능한지 체크 해야 함
                //#region Swagger Antiforgery 사용하도록 설정
                //var antiforgery = app.ApplicationServices.GetRequiredService<IAntiforgery>();
                //app.Use((context, next) =>
                //{
                //    var requestPath = context.Request.Path.Value;

                //    if (string.Equals(requestPath, "/", StringComparison.OrdinalIgnoreCase)
                //        || string.Equals(requestPath, "/index.html", StringComparison.OrdinalIgnoreCase))
                //    {
                //        var tokenSet = antiforgery.GetAndStoreTokens(context);
                //        context.Response.Cookies.Append("XSRF-TOKEN", tokenSet.RequestToken!,
                //            new CookieOptions { HttpOnly = false });
                //    }

                //    return next(context);
                //});
                //#endregion
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("v1/swagger.json", "V1");
                options.RoutePrefix = string.Empty;
            });
        }
    }
}
