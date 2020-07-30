using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalileuszSchool.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebPWrecover.Services;
using GalileuszSchool.Services;
using GalileuszSchool.Repository;
using GalileuszSchool.Areas.Admin.Controllers;
using GalileuszSchool.Models.ModelsForNormalUsers;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using GalileuszSchool.Services.Facebook;

namespace GalileuszSchool
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddControllersWithViews();

            services.AddSession(options =>
            {
                //options.IdleTimeout = TimeSpan.FromSeconds(2);
            });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            //Register database in ConfigurationServices
            services.AddDbContext<GalileuszSchoolContext>(options => options
                    .UseSqlServer(Configuration.GetConnectionString("GalileuszSchoolContext")));

            services.AddIdentity<AppUser, IdentityRole>(options => {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
                    .AddEntityFrameworkStores<GalileuszSchoolContext>()
                    .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddRazorPages();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<StudentsController>();
            services.AddTransient<TeachersController>();
            //services.AddScoped<ICoursesRepository, CoursesRepository>();
            //it here in case specific modifications have to be made to any model
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var googleAuth = Configuration.GetSection("Authentication:Google");

                    options.ClientId = googleAuth["ClientId"];
                    options.ClientSecret = googleAuth["ClientSecret"];
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    options.Events = new OAuthEvents()
                    {
                        OnTicketReceived = ctx =>
                        {
                            var username =
                                ctx.Principal.FindFirstValue(ClaimTypes
                                                                 .NameIdentifier);
                            //TODO: Add user check/creation here 

                            ctx.Response.Redirect("/");
                            ctx.HandleResponse();
                            return Task.CompletedTask;
                        }
                    };
                });
            var facebookAuthSettings = new FacebookAuthSettings();
            Configuration.GetSection("Authentication:FacebookAuthSettings").Bind(facebookAuthSettings);
            //Configuration.Bind(nameof(FacebookAuthSettings), facebookAuthSettings);
            services.AddSingleton(facebookAuthSettings);
            services.AddHttpClient();
            services.AddSingleton<IFacebookAuthService, FacebookAuthService>();
            services.AddSingleton<IAppSettingsService, AppSettingsService>(e => Configuration.GetSection(nameof(AppSettingsService)).Get<AppSettingsService>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            //app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Register}/{id?}");
            });
        }
    }
}
