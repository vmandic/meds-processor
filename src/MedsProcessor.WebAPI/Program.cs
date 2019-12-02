using System.Threading.Tasks;
using MedsProcessor.WebAPI.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MedsProcessor.WebAPI
{
    public class Program
	{
		public static async Task Main(string[] args)
		{
			var host = CreateWebHostBuilder(args).Build();

			await host.PreRunLogicAsync();
			await host.RunAsync();
		}

		private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost
			.CreateDefaultBuilder(args)
			.UseStartup<Startup>();
	}
}