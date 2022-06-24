using Boilerplate.Web.Context;
using Boilerplate.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boilerplate.Web.Controllers;
using Boilerplate.Web.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Microsoft.AspNetCore.Identity;
using Hellang.Middleware.ProblemDetails.Mvc;
using Newtonsoft.Json.Converters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using Boilerplate.Web.Models;
using AutoMapper.Data;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Boilerplate.Web
{
    /// <summary>
    /// Boilerplate Web Program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Configuration
        /// </summary>
        public static IConfiguration? Configuration { get; private set; }

        /// <summary>
        /// Environment
        /// </summary>
        public static IWebHostEnvironment? CurrentEnvironment { get; private set; }

        /// <summary>
        /// Program main constructor
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
#if DEBUG
            var environmentName = Environments.Development;
#else
            var environmentName = Environments.Production;
#endif
            ServerStart(args, isService: false, environmentName: environmentName).Run();
        }

        /// <summary>
        /// Start Web Server
        /// </summary>
        /// <param name="args"></param>
        /// <param name="isService">서비스에서 실행 여부</param>
        /// <param name="environmentName">환경모드</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static WebApplication ServerStart(string[] args, bool isService = false, string environmentName = "Production")
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = Directory.GetCurrentDirectory(), 
                EnvironmentName = environmentName
            });

            Configuration = builder.Configuration;
            CurrentEnvironment = builder.Environment;

            builder.Services.AddProblemDetails(ConfigureProblemDetails);

#region Response Compression
            builder.Services.AddResponseCompression(options =>
            {
                //https://docs.microsoft.com/ko-kr/aspnet/core/performance/response-compression?view=aspnetcore-6.0
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });
            builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.SmallestSize;
            });
#endregion

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<BoilerplateContext>(options =>
            {
                // Sqlite를 사용할 때
                options.UseSqlite(connectionString);

                // Memory DB를 사용할 때
                //options.UseInMemoryDatabase("BoilerplateData.db");

                // MSSQL 사용 시 
                //options.UseSqlServer(connectionString);
            });
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

#region Set IdentityFramework
            builder.Services.AddDefaultIdentity<IdentityWebUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<BoilerplateContext>();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
#endregion

#region MVC & Razor pages
            builder.Services.AddControllersWithViews(options => {
                // Global Antiforgery Options
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            // Adds MVC conventions to work better with the ProblemDetails middleware.
            .AddProblemDetailsConventions()
            .ConfigureApiBehaviorOptions(options => {
                //options.SuppressConsumesConstraintForFormFileParameters = true;
                //options.SuppressInferBindingSourcesForParameters = true;
                //options.SuppressModelStateInvalidFilter = true;

                //options.InvalidModelStateResponseFactory = context =>
                //    new BadRequestObjectResult(context.ModelState)
                //    {
                //        ContentTypes =
                //        {
                //            // using static System.Net.Mime.MediaTypeNames;
                //            Application.Json
                //        }
                //    };

                // 에러 페이지 매핑
                //https://docs.microsoft.com/ko-kr/aspnet/core/web-api/handle-errors?view=aspnetcore-6.0#implement-problemdetailsfactory
                options.ClientErrorMapping[StatusCodes.Status404NotFound].Link = "https://httpstatuses.com/404";
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
                //options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
                options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddRazorPages()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
                    //options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
                    options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
                });

            if (isService)
            {
                var assembly = typeof(Program).Assembly;
                builder.Services.AddControllersWithViews().AddApplicationPart(assembly);
            }
#endregion

#region Swagger
            builder.Services.AddSwaggerGen(options =>
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
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("/auth-server/connect/authorize", UriKind.Relative),
                            Scopes = new Dictionary<string, string>
                            {
                                { "readAccess", "Access read operations" },
                                { "writeAccess", "Access write operations" }
                            }
                        }
                    }
                });
#endregion

                // 주석 연동이 되도록 세팅
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            builder.Services.AddSwaggerGenNewtonsoftSupport(); // explicit opt-in - needs to be placed after AddSwaggerGen()
            //services.AddSwaggerExamples();
            builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
#endregion

#region AutoMapper
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.AddCollectionMappers();
            //    cfg.SetGeneratePropertyMaps<GenerateEntityFrameworkPrimaryKeyPropertyMaps<DB>>();
            //    // Configuration code
            //});

            builder.Services.AddAutoMapper((serviceProvider, automapper) => {
                // todo 검토 중
                //automapper.AddCollectionMappers();
                //automapper.UseEntityFrameworkCoreModel<BoilerplateContext>(serviceProvider);

                //automapper.AddExpressionMapping();
                automapper.AddDataReaderMapping();
                automapper.CreateMap<TodoItem, TodoItemDTO>();
                automapper.CreateMap<TodoItemDTO, TodoItem>();
            }, typeof(BoilerplateContext).Assembly);
#endregion


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();

#region 개발 시 에러 페이지 리턴 형식 지정
                // 에러시 개발 페이지 형식으로 에러 페이지 표시
                //app.UseDeveloperExceptionPage();
                // 에러시 json 형식으로 에러 정보 표시
                app.UseProblemDetails();
#endregion

                app.UseSwagger();
                app.UseSwaggerUI(options => {
                    // Swagger에서 사용하는 CSS 추가
                    //options.InjectStylesheet("/swagger-ui/custom.css");
                });

                // todo : Swagger에서 Antiforgery 사용가능한지 체크 해야 함
#region Swagger Antiforgery 사용하도록 설정
                //var antiforgery = app.Services.GetRequiredService<IAntiforgery>();
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
#endregion

                // /api-docs ReDoc 문서도 추가로 등록 함
                app.UseReDoc(c =>
                {
                    c.DocumentTitle = "REDOC API Documentation";
                    c.SpecUrl = "/swagger/v1/swagger.json";
                });

                // 에러시 개발 페이지 형식으로 에러 페이지 표시
                //app.UseExceptionHandler("/error-development");
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                //https://docs.microsoft.com/ko-kr/aspnet/core/performance/response-compression?view=aspnetcore-6.0
                app.UseResponseCompression();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
                endpoints.MapSwagger();
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("v1/swagger.json", "V1");
                options.RoutePrefix = string.Empty;

                // xsrf token processing
                //options.UseRequestInterceptor("(req) => { req.headers['XSRF-TOKEN'] = localStorage.getItem('xsrf-token'); return req; }");
                options.UseRequestInterceptor("(req) => { debugger; req.headers['X-XSRF-TOKEN'] = (await cookieStore.get('XSRF-TOKEN')).value; return req; }");

                // oauth
                //options.OAuthClientId("test-id");
                //options.OAuthClientSecret("test-secret");
                //options.OAuthRealm("test-realm");
                //options.OAuthAppName("test-app");
                //options.OAuthScopeSeparator(" ");
                //options.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "foo", "bar" } });
                //options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });

            CreateDbIfNotExistsAsync(app).Wait();

            app.Logger.LogInformation("-----------------------------------------------------------------");
            app.Logger.LogInformation("-----------------------------------------------------------------");
            app.Logger.LogInformation($"ApplicationName: {builder.Environment.ApplicationName}");
            app.Logger.LogInformation($"Environment Mode: {builder.Environment.EnvironmentName}");
            app.Logger.LogInformation($"ContentRoot Path: {builder.Environment.ContentRootPath}");
            app.Logger.LogInformation($"WebRootPath: {builder.Environment.WebRootPath}");
            app.Logger.LogInformation("-----------------------------------------------------------------");
            app.Logger.LogInformation("-----------------------------------------------------------------");

            if (isService)
            {
                app.Urls.Add("https://localhost:7298");
                app.Urls.Add("http://localhost:5298");
            }

            return app;
        }

        /// <summary>
        /// Create Db If Not Exists
        /// </summary>
        /// <param name="app"></param>
        public static async Task CreateDbIfNotExistsAsync(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<BoilerplateContext>();
                    await DbInitializer.InitializeAsync(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }

            await Task.Delay(0);
        }

        private static void ConfigureProblemDetails(ProblemDetailsOptions options)
        {
            // Only include exception details in a development environment. There's really no nee
            // to set this as it's the default behavior. It's just included here for completeness :)
            options.IncludeExceptionDetails = (ctx, env) => CurrentEnvironment.IsDevelopment() || CurrentEnvironment.IsStaging();

            // use custom exception class
            //setup.Map<ProductCustomException>(exception => new ProblemDetails
            //{
            //    Title = exception.Title,
            //    Detail = exception.Detail,
            //    Status = StatusCodes.Status500InternalServerError,
            //    Type = exception.Type,
            //    Instance = exception.Instance,
            //    Extensions[“additionalInfo”] = exception.AdditionalInfo // this already creates a property and can be reused as much as necessary
            //});

            // Custom mapping function for FluentValidation's ValidationException.
            //options.MapFluentValidationException();

            // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
            // This is useful if you have upstream middleware that needs to do additional handling of exceptions.
            options.Rethrow<NotSupportedException>();

            // This will map NotImplementedException to the 501 Not Implemented status code.
            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

            // This will map HttpRequestException to the 503 Service Unavailable status code.
            options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

            // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
            // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        }
    }
}
