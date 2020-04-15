using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace AuctionMvc
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
        .ConfigureLogging((hostingContext, logging) =>
        {
          logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
          logging.AddDebug();
          logging.AddNLog();
        })
        .UseStartup<Startup>();
  }
}
