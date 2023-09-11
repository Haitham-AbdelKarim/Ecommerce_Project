using Microsoft.AspNetCore.Authentication.Cookies;

namespace Ecommerce_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication("cookie").AddCookie("cookie", options =>
            {
                options.Cookie.Name = "cookie";
                options.LoginPath = "/User/Login";
                options.AccessDeniedPath = "/Home/AccessDenied";
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("isAdmin", "True"));
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Customer", policy => policy.RequireClaim("isAdmin", "False"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}