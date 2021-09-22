using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using ShopWeb.Data;
using System.IO;
using Newtonsoft.Json.Linq;
using ShopWeb.ViewModels;
using Blazored.LocalStorage;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Connections;

namespace ShopWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            bool IsDevelopment = false;

            string JsonString;
            if (IsDevelopment)
            {
                JsonString = File.ReadAllText("appsettings.Development.json");                
            }
            else
            {
                JsonString = File.ReadAllText("appsettings.json");
            }

            JObject JObject = JObject.Parse(JsonString);

            string ConnectionString = JObject.SelectToken("ConnectionStrings").SelectToken("DefaultConnection").ToString();

            ShopWebContext.ConnectionString = ConnectionString;

            services.AddDataProtection()
                .PersistKeysToDbContext<ShopWebContext>();

            services.AddDbContext<ShopWebContext>();

            services.AddBlazoredLocalStorage();
                
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<User, IdentityRole>(options => {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ShopWebContext>()
            .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider)
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddDefaultUI();

            services.AddAuthentication();
            services.AddAuthorization();

            services.AddRazorPages();                

            services.AddServerSideBlazor();

            services.AddSignalR(e => {
                e.MaximumReceiveMessageSize = null;
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;                
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute
                (
                    name: "registration",
                    pattern: "Реєстрація/{id?}",
                    defaults: new { controller = "Registration", action = "Index" }
                );
                endpoints.MapControllerRoute
                (
                    name: "category",
                    pattern: "Продукція/{category?}/{subcategory?}",
                    defaults: new { controller = "Products", action = "List" }
                );
                endpoints.MapControllerRoute
                (
                    name: "cart",
                    pattern: "Cart/{id?}",
                    defaults: new { controller = "Cart", action = "Index" }
                );
                endpoints.MapControllerRoute
                (
                    name: "checkout",
                    pattern: "Checkout/{id?}",
                    defaults: new { controller = "Checkout", action = "Index" }
                );
                endpoints.MapControllerRoute
                (
                    name: "orders",
                    pattern: "Orders/{action}",
                    defaults: new { controller = "Orders", action = "Index" }
                );
                endpoints.MapControllerRoute
                (
                    name: "roles",
                    pattern: "Roles/{action}",
                    defaults: new { controller = "Roles", action = "Index"}
                );
                endpoints.MapControllerRoute
                (
                    name: "menu",
                    pattern: "Menu/{action}",
                    defaults: new { controller = "Menu", action = "Index" }
                );

                endpoints.MapControllerRoute
                (
                    name: "manager",
                    pattern: "Manager/{action}",
                    defaults: new { controller = "Manager", action = "Index" }
                );
                endpoints.MapControllerRoute
                (
                    name: "product_page",
                    pattern: "Product/{Id:int}",
                    defaults: new { controller = "ProductPage", action = "Index" }
                );
                endpoints.MapControllerRoute
                (
                    name: "slider",
                    pattern: "Slider/{action}",
                    defaults: new { controller = "Menu", action = "SliderIndex" }
                );
                endpoints.MapControllerRoute
                (
                    name: "default",
                    pattern: "{MenuName?}/{SubMenuName?}",
                    defaults: new { controller = "Main", action = "Main" }
                );


                endpoints.MapRazorPages();

                endpoints.MapBlazorHub();
            });
        }
    }
}
