﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using NLog.Web;
using OnlineSalesCore.Jobs;
using OnlineSalesCore.Cache;
using OnlineSalesCore.DTO;
using OnlineSalesCore.Models;
using OnlineSalesCore.Logic;
using OnlineSalesCore.Logic.Impl;
using OnlineSalesCore.Options;
using OnlineSalesCore.Queries;
using OnlineSalesCore.Services;
using OnlineSalesTool.AuthToken;
using System;
using System.Linq;
using System.Text;
using System.IO;

namespace OnlineSalesTool
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment { get; private set;}
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddServices(services);
            AddJobs(services);
            AddOptions(services);
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
                options.MimeTypes = new[] {
                    "text/css",
                    "application/javascript" };
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Fastest;
            });


            //Enforce SSL
            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());
            //});
            //https://github.com/aspnet/Mvc/issues/4842

            services.AddMvc().AddJsonOptions(options =>
            {
                //Solve auto camel case prop names
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //Ignore loop ref of object contains each other
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            //Nlog
            env.ConfigureNLog("NLog.config");
            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();
            //Webserver stuff
            app.UseAuthentication();
            app.UseExceptionHandler("/Home/Error");
            app.UseStatusCodePages();
            app.UseResponseCompression();
            // app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            //Fall back for SPA
            app.MapWhen(context => context.Request.Path.Value.StartsWith("/App"), builder =>
            {
                builder.UseMvc(routes =>
                {
                    routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Home", action = "Index" });
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
                routes.MapRoute(
                   name: "api",
                   template: "API/{controller}/{action}");
                // This causes wrong URL to fallback to app which feels weird
                // routes.MapSpaFallbackRoute(
                //     name: "spa-fallback",
                //     defaults: new { controller = "Home", action = "Index" });
            });
        }
        public void AddServices(IServiceCollection services)
        {
            //Singleton
            services.AddSingleton<IMailQueue, MailQueue>();
            //Inject db context
            services.AddDbContext<OnlineSalesContext>(o => o.UseSqlServer(Configuration.GetConnectionString("Default")));
            //Inject JWT factory
            services.AddSingleton<IJwtFactory, JwtFactory>();
            //Inject HttpAccessor
            services.AddHttpContextAccessor();
            //Inject service
            // services.AddScoped<IIndusService, IndusService>();
            services.AddScoped<IIndusService, IndusService>();
            services.AddScoped<IContextAwareService, ContextAwareService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPosService, PosService>();
            services.AddScoped<ICaseService, CaseService>();
            services.AddScoped<IRoleCache, RoleCache>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IScheduleMatcher, SimpleScheduleMatcher>();
            services.AddScoped<ILdapAuth, LdapAuth>();
            services.AddScoped<ListQuery<Pos, PosDTO>, PosListQuery>();
            services.AddScoped<ListQuery<AppUser, AppUserDTO>, UserListQuery>();
            services.AddScoped<ListQuery<OnlineOrder, CaseDTO>, CaseListQuery>();
            services.AddScoped<IAPIAuth, APIAuth>();
            services.AddScoped<IDMCLService, DMCLService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHtmlComposer, HtmlComposer>();
            services.AddScoped<IMailerService, MailerService>();
        }
        public void AddJobs(IServiceCollection services)
        {
            services.AddSingleton<IHostedService, SyncJob>();
            services.AddSingleton<IHostedService, MailJob>();
        }
        private void AddOptions(IServiceCollection services)
        {
            var jwtSetting = Configuration.GetSection(nameof(JwtIssuerOptions));
            var authSetting = Configuration.GetSection(nameof(WindowsAuthOptions));
            var genOptions = Configuration.GetSection(nameof(GeneralOptions));
            var apiAuthOptions = Configuration.GetSection(nameof(APIAuthOptions));
            var syncOptions = Configuration.GetSection(nameof(SyncOptions));
            var mailerSection = Configuration.GetSection(nameof(MailerOptions));
            var receiverSection = mailerSection.GetSection(nameof(MailerOptions.Receivers));
            var shiftScheduleOptions = Configuration.GetSection(nameof(ShiftScheduleOptions));
            var caseOptions = Configuration.GetSection(nameof(CaseOptions));
            var indusOptions = Configuration.GetSection(nameof(IndusOptions));
            //Indus options
            services.Configure<IndusOptions>(o =>
            {
                o.ConnectionString = indusOptions[nameof(IndusOptions.ConnectionString)];
                o.ContractDetailScript = indusOptions[nameof(IndusOptions.ContractDetailScript)];
            });
            //Shift scheudle
            services.Configure<ShiftScheduleOptions>(o =>
            {
                o.Start = int.Parse(shiftScheduleOptions[nameof(ShiftScheduleOptions.Start)]);
                o.End = int.Parse(shiftScheduleOptions[nameof(ShiftScheduleOptions.End)]);
            });
            //Case options
            services.Configure<CaseOptions>(o =>
            {
                o.NoNatIdCheck = caseOptions[nameof(CaseOptions.NoNatIdCheck)] == "1";
            });
            //Mailer
            services.Configure<MailerOptions>(o =>
            {
                o.Server = mailerSection[nameof(MailerOptions.Server)];
                o.Port = int.Parse(mailerSection[nameof(MailerOptions.Port)]);
                o.Username = mailerSection[nameof(MailerOptions.Username)];
                o.Pwd = mailerSection[nameof(MailerOptions.Pwd)];
                o.Suffix = mailerSection[nameof(MailerOptions.Suffix)];
                o.Receivers = receiverSection.GetChildren().Select(c => c.Value);
                o.Disabled = mailerSection[nameof(MailerOptions.Disabled)] == "1";
            });
            //Inject options
            //API auth
            services.Configure<APIAuthOptions>(o =>
            {
                o.Pwd = apiAuthOptions[nameof(APIAuthOptions.Pwd)];
                o.NoAuth = apiAuthOptions[nameof(APIAuthOptions.NoAuth)] == "1";
            });
            //Gen option
            services.Configure<GeneralOptions>(o =>
            {
                //Gen options go here
            });
            //Sync options
            services.Configure<SyncOptions>(o =>
            {
                o.Delay = int.Parse(syncOptions[nameof(SyncOptions.Delay)]);
                o.Disabled = syncOptions[nameof(SyncOptions.Disabled)] == "1";
            });
            //JWT option
            var secret = Configuration.GetSection(nameof(JwtIssuerOptions)).GetValue<string>("Secret");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            services.Configure<JwtIssuerOptions>(o =>
            {
                o.Issuer = jwtSetting[nameof(JwtIssuerOptions.Issuer)];
                o.Audience = jwtSetting[nameof(JwtIssuerOptions.Audience)];
                o.SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            });
            //Auth option
            services.Configure<WindowsAuthOptions>(o =>
            {
                o.NoPwdCheck = authSetting[nameof(WindowsAuthOptions.NoPwdCheck)] == "1";
                o.Issuer = authSetting[nameof(WindowsAuthOptions.Issuer)];
                o.Domain = authSetting[nameof(WindowsAuthOptions.Domain)];
            });
            var tokenValidationParameters = new TokenValidationParameters
            {
                //ValidateIssuer = true,
                ValidIssuer = jwtSetting[nameof(JwtIssuerOptions.Issuer)],
                //ValidateAudience = true,
                ValidAudience = jwtSetting[nameof(JwtIssuerOptions.Audience)],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromDays(1)
            };
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.ClaimsIssuer = jwtSetting[nameof(JwtIssuerOptions.Issuer)];
                o.TokenValidationParameters = tokenValidationParameters;
                o.SaveToken = true;
            });
        }
    }
}
