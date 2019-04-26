using System.Threading.Tasks;
using MedsProcessor.WebAPI.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MedsProcessor.WebAPI
{
    public class Program
	{
		public static async Task Main(string[] args)
		{
			IWebHost host = CreateWebHostBuilder(args).Build();
			var processor = host.Services.GetService<HzzoDataProcessor>();

			#pragma warning disable
			// no need to wait for processor to finish
			processor.Run();
			#pragma warning enable

			await host.RunAsync();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
			.UseStartup<Startup>();
	}
}