using System.Threading.Tasks;
using AspNetCoreRateLimit;
using MedsProcessor.WebAPI.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MedsProcessor.WebAPI.Extensions
{
	public static class IWebHostExtensions
	{
		public static async Task PreRunLogicAsync(this IWebHost host)
		{
			// ref: https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/Version-3.0.0-Breaking-Changes
			using(var scope = host.Services.CreateScope())
			{
				// get the IpPolicyStore instance
				var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();

				// seed IP data from appsettings
				await ipPolicyStore.SeedAsync();
			}

			var processor = host.Services.GetService<HzzoDataProcessor>();

#pragma warning disable
			// no need to wait for processor to finish
			processor.RunAsync();
#pragma warning enable
		}
	}
}