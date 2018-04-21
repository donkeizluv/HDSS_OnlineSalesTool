using OnlineSalesTool.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Service;
using OnlineSalesTool.Repository;
using System;
using System.Text;

namespace OnlineSalesTool
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public string SecretKey
        {
            get
            {
                return Configuration.GetSection(nameof(JwtIssuerOptions)).GetValue<string>("Secret");
            }
        }
        private readonly SymmetricSecurityKey _signingKey;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Inject db context
            services.AddDbContext<OnlineSalesContext>(o => o.UseSqlServer(Configuration.GetConnectionString("Default")));
            //Inject JWT factory
            services.AddSingleton<IJwtFactory, JwtFactory>();
            //Inject config
            services.AddSingleton(Configuration); //Define specific option then inject use IOptions
            //Inject repos
            services.AddTransient<IScheduleRepository, ScheduleRepository>();
            //Inject INDUS
            //services.AddSingleton<IIndusAdapter>(IndusFactory.GetIndusInstance(Configuration,
            //    File.ReadAllText($"{Program.ExeDir}\\{Configuration.GetSection("Indus").GetValue<string>("QueryFileName")}")));
            //Inject HttpAccessor
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            //Inject resolver service
            services.AddTransient<IUserResolver, UserResolver>();
            //Get setting section
            var jwtSetting = Configuration.GetSection(nameof(JwtIssuerOptions));
            //Inject option
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtSetting[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtSetting[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                //ValidateIssuer = true,
                ValidIssuer = jwtSetting[nameof(JwtIssuerOptions.Issuer)],
                
                //ValidateAudience = true,
                ValidAudience = jwtSetting[nameof(JwtIssuerOptions.Audience)],
                
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtSetting[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            //policy
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(AbilityList.Create, policy => policy.RequireClaim(AppConst.Ability, AbilityList.Create));
            //    options.AddPolicy(AbilityList.Delete, policy => policy.RequireClaim(AppConst.Ability, AbilityList.Delete));
            //    options.AddPolicy(AbilityList.Update, policy => policy.RequireClaim(AppConst.Ability, AbilityList.Update));
            //    options.AddPolicy(AbilityList.Download, policy => policy.RequireClaim(AppConst.Ability, AbilityList.Download));
            //    options.AddPolicy(AbilityList.ManageUser, policy => policy.RequireClaim(AppConst.Ability, AbilityList.ManageUser));
            //});

            //Compression
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                //Everything else is too small to compress
                options.MimeTypes = new[] { "text/css", "application/javascript" };
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Fastest;
            });


            //enforce SSL
            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());
            //});
            //https://github.com/aspnet/Mvc/issues/4842

            services.AddMvc().AddJsonOptions(options =>
            {
                //solve auto camel case prop names
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //ignore loop ref of object contains each other
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            //enforce SSL
            //app.UseRewriter(new RewriteOptions().AddRedirectToHttps((int)HttpStatusCode.Redirect, 44395));

            //Use this in PROD
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseBrowserLink();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}

            app.UseResponseCompression();
            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
                routes.MapRoute(
                   name: "api",
                   template: "API/{controller}/{action}");
                //Use this to fallback route in case of using vue router heavily
                //Install - Package Microsoft.AspNetCore.SpaServices
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

    }
}
