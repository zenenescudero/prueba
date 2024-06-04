using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace ArzyzWeb
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
            services.AddControllersWithViews();
            services.AddDistributedMemoryCache();
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
                options.ExpireTimeSpan = TimeSpan.FromDays(5.0);
            });

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

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
            }
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Files")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Files"));
            }
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Temp")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Temp"));
            }
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "PhotosUsers")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "PhotosUsers"));
            }

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Logs")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Logs"));
            }

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
                RequestPath = new PathString("/Files")
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), @"Temp")),
                RequestPath = new PathString("/Temp")
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
              Path.Combine(Directory.GetCurrentDirectory(), @"PhotosUsers")),
                RequestPath = new PathString("/PhotosUsers")
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), @"Logs")),
                RequestPath = new PathString("/Logs")
            });


            app.UseRouting();

            app.Use(async (context, next) =>
            {
                try
                {
                    if (context.Request.Query.ContainsKey("ReturnUrl"))
                    {
                        context.Response.Redirect("/Account/Login");
                    }
                    // Do work that doesn't write to the Response.
                    await next.Invoke();
                    // Do logging or other work that doesn't write to the Response.
                }
                catch (Exception ex)
                {
                    Models.Helpers.LogRegister($"{context.Request.Method} {context.Request.Path}", $"{ex.Message} - Inner -> {ex.InnerException?.Message} {ex.StackTrace}");


                    // Configurar la respuesta en formato JSON
                    context.Response.StatusCode = 500; // Código de estado de error interno del servidor
                    context.Response.ContentType = "application/json";

                    var errorResponse = new
                    {
                        error = true,
                        mensaje = "Se presento un error revisa el log."
                    };

                    // Escribir la respuesta JSON
                    await context.Response.WriteAsJsonAsync(errorResponse);
                }
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
