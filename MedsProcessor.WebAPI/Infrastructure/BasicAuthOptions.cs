using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace MedsProcessor.WebAPI.Infrastructure
{
	public class BasicAuthOptions
	{
		public IEnumerable<string> AuthorizedRoutes { get; private set; } = Enumerable.Empty<string>();
		public string Username { get; set; }
		public string Password { get; set; }
		public bool AuthorizeLocalRequest { get; set; } = false;
		public bool ShouldAuthorizeLocalRequests(HttpContext context) =>
			AuthorizeLocalRequest || !IsLocalRequest(context);

		public void AuthorizeRoutes(params string[] routes) =>
			AuthorizedRoutes = routes.AsEnumerable();

		public bool ShouldAuthorizeRequestPath(PathString httpRequestPath) =>
			AuthorizedRoutes.Any(x =>
				httpRequestPath.StartsWithSegments(x, StringComparison.OrdinalIgnoreCase));

		private static bool IsLocalRequest(HttpContext context) =>
			(context.Connection.RemoteIpAddress == null && context.Connection.LocalIpAddress == null) ||
			(context.Connection.RemoteIpAddress.Equals(context.Connection.LocalIpAddress)) ||
			IPAddress.IsLoopback(context.Connection.RemoteIpAddress);
	}
}