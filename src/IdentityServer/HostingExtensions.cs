using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using identityserver;
using IdentityServer.Common;
using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // uncomment if you want to add a UI
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();

        var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var identityConnectionString = builder.Configuration.GetConnectionString("IdentityDefaultConnection");

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.WithOrigins("http://localhost:4200") // Specify the allowed origins
               .AllowAnyHeader()                   // Allow any headers
               .AllowAnyMethod()                   // Allow any HTTP methods
               .AllowCredentials()
               );
        });

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(identityConnectionString));
        
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = false; // Don't require digits
            options.Password.RequiredLength = 1;   // Allow passwords with a minimum length of 1 character
            options.Password.RequireNonAlphanumeric = false; // Don't require non-alphanumeric characters
            options.Password.RequireUppercase = false; // Don't require uppercase letters
            options.Password.RequireLowercase = false; // Don't require lowercase letters
            options.Password.RequiredUniqueChars = 1; // A
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;


                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
             .AddConfigurationStore(options =>
             {
                 options.ConfigureDbContext = b => b.UseSqlite(connectionString,
                     sql => sql.MigrationsAssembly(migrationsAssembly));
             })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlite(connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryApiResources(Config.Apis)
            .AddInMemoryClients(Config.Clients)
            .AddProfileService<CustomProfileService>();
            //.AddTestUsers(TestUsers.Users);

        builder.Services.ConfigureApplicationCookie(config =>
        {
            config.Cookie.Name = "Identity.Cookie";
            //config.LoginPath = "/Account/Login"; // Redirects to the login page when unauthenticated
            config.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax; // Lax is appropriate for most OAuth scenarios
            config.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use Always if using HTTPS
        });


        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("CorsPolicy");

        //app.UseCookiePolicy(new CookiePolicyOptions
        //{
        //    HttpOnly = HttpOnlyPolicy.None,
        //    MinimumSameSitePolicy = SameSiteMode.None,
        //    Secure = CookieSecurePolicy.Always
        //});

        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
        });

        // CSP configuration
        if (app.Environment.IsDevelopment())
        {
            // This middleware adds the CSP header for development environment
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy",
                    "default-src 'self'; connect-src 'self' wss://localhost:44329 ws://localhost:60425 http://localhost:60425;");
                await next();
            });
        }
        else
        {
            // This middleware adds the CSP header for production environment
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy",
                    "default-src 'self'; connect-src 'self';");
                await next();
            });
        }
        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); // Maps attribute-routed controllers
        });

        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}

