using Library.Data;
using Library.Services.Books;
using Library.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Context;

namespace Library
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddDb();

            builder.Services.AddScoped<IRoleStore<AppRole>, AppRoleStore>();
            builder.Services.AddScoped<IUserStore<AppUser>, AppUserStore>();
            builder.Services.AddScoped<IUserRoleStore<AppUser>, AppUserStore>();
            
            builder.Services.AddDefaultIdentity<AppUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
//Add configs for minimum password
                options.Password.RequiredLength = 7;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
            })
            .AddRoles<AppRole>()
            .AddDefaultTokenProviders();

            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            builder.Services.AddControllersWithViews();

            //Add support to logging with SERILOG
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddSingleton<IBooksService, BooksService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/home/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (httpContext, next) =>
            {
                if (httpContext.User.Identity.IsAuthenticated)
                {
                    //Get username  
                    LogContext.PushProperty("User", httpContext.User.Identity.Name);
                }

                await next.Invoke();
            });


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
