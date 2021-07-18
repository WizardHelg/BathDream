using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using BathDream.Data;
using BathDream.Models;
using BathDream.Validators;
using BathDream.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.IO;

namespace BathDream
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
            services.AddMvc();
            services.AddSingleton<SMSConfirmator>();
            services.AddTransient<EmailSender>();
            services.AddTransient<SMSSender>();
            services.AddSingleton<PDFConverter>();

            var context = new CustomAssemblyLoadContext();
            string libwkhtmlPath;
#if DEBUG
            libwkhtmlPath = "x64";
#else
            libwkhtmlPath = "x32";
#endif
            context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "lib", libwkhtmlPath, "libwkhtmltox.dll"));

            //services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            //services.AddSingleton(PDFConverter);

            services.AddTransient<IPasswordValidator<User>, CustomPasswordValidator>(serv => new CustomPasswordValidator()
            {
                RequiredLength = 6,
                RequireNonAlphanumeric = true,
                RequireDigit = true,
                RequireUppercase = true,
                RequireLowercase = true
            });

            services.AddTransient<IUserValidator<User>, CustomUserValidator>(serv => new CustomUserValidator()
            {
                AllowedUserNameCharactersReqEx = @"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$",
                RequireUniqueEmail = false
            });

            services.AddDbContext<DBApplicationaContext>(options =>
            {
#if DEBUG
                options.UseSqlServer(Configuration.GetConnectionString("TestConnection"));
#else
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
#endif
            });

                services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
                .AddEntityFrameworkStores<DBApplicationaContext>()
                .AddDefaultTokenProviders();

            services.AddSignalR();

            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddSingleton(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic }));
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
                app.UseExceptionHandler("/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapRazorPages();
            });
        }
    }
}
