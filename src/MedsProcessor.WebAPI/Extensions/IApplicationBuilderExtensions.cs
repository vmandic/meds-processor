using System;
using MedsProcessor.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Builder;

namespace MedsProcessor.WebAPI.Extensions
{
	public static class IApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseBasicAuthentication(
			this IApplicationBuilder app,
			Action<BasicAuthOptions> configureBasicAuthOpts)
		{
			var opts = new BasicAuthOptions();
			configureBasicAuthOpts(opts);

			return app.UseMiddleware<BasicAuthMiddleware>(opts);
		}
	}
}