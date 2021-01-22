using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ShareUp.Services;
using ShareUp.Models;

namespace ShareUp
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
            services.Configure<TransactionDatabaseSettings>(
                Configuration.GetSection(nameof(TransactionDatabaseSettings)));

            services.AddSingleton<ITransactionDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<TransactionDatabaseSettings>>().Value);

            services.Configure<AdminSettings>(
                Configuration.GetSection(nameof(AdminSettings)));

            services.AddSingleton<IAdminSettings>(sp =>
                sp.GetRequiredService<IOptions<AdminSettings>>().Value);

            services.Configure<AppSettings>(
                Configuration.GetSection(nameof(AppSettings)));

            services.AddSingleton<IAppSettings>(sp =>
                sp.GetRequiredService<IOptions<AppSettings>>().Value);

            services.AddSingleton<AdminService>();
            services.AddSingleton<TransactionService>();
            services.AddSingleton<AccountService>();

            services.AddAuthentication("CookieAuthentication")
                .AddCookie("CookieAuthentication", config =>
                {
                    config.Cookie.Name = "LoginCookie";
                    config.LoginPath = "/Account";
                });

            services.AddRazorPages();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
