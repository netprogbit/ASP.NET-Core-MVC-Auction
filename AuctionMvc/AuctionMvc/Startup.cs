using AuctionMvc.Helpers;
using AuctionMvc.Hubs;
using AuctionMvc.Services;
using AuctionMvc.Settings;
using DataLayer;
using DataLayer.DbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IO;

namespace AuctionMvc
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
      StaticConfig = configuration;
    }

    public IConfiguration Configuration { get; }
    public static IConfiguration StaticConfig { get; private set; } // For use by static classes

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
      services.AddSession();

      services.AddAuthentication(o =>
      {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(o =>
      {
        o.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = JwtHelper.GetSymmetricSecurityKey()
        };
      });

      services.AddDbContext<AuctionDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
      services.AddScoped<UnitOfWork>();
      services.AddScoped<AuthService>();
      services.AddScoped<AuctionService>();

      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => false;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      services.AddSignalR();
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseSession();

      //Add token to each HTTP request header
      app.Use(async (context, next) =>
      {
        var token = context.Session.GetString("token");

        if (!string.IsNullOrEmpty(token))
          context.Request.Headers.Add("Authorization", "Bearer " + token);

        await next();
      });

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error/Error");
        app.UseHsts();
      }

      app.UseStatusCodePagesWithReExecute("/Error/HttpError", "?code={0}");
      app.UseHttpsRedirection();
      app.UseDefaultFiles();
      app.UseStaticFiles();
      app.UseCookiePolicy();
      app.UseAuthentication();

      app.UseFileServer(new FileServerOptions()
      {
        FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),
        RequestPath = "/node_modules",
        EnableDirectoryBrowsing = false
      });

      app.UseSignalR(routes =>
      {
        routes.MapHub<AuctionHub>("/auction");
      });

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller=Auction}/{action=Index}/{id?}");
      });

      FileHelper.HostingEnvironment = env; // Set hosting environment for file helper
    }
  }
}
