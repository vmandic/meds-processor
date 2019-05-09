using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MedsProcessor.WebAPI.Infrastructure
{
	public class BasicAuthMiddleware
	{
		private readonly RequestDelegate next;
		private readonly BasicAuthOptions options;

		public BasicAuthMiddleware(RequestDelegate next, BasicAuthOptions options)
		{
			this.options = options;
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (options.ShouldAuthorizeRequestPath(context.Request.Path)
			&& options.ShouldAuthorizeLocalRequests(context))
			{
				string authHeader = context.Request.Headers["Authorization"];
				if (authHeader != null && authHeader.StartsWith("Basic "))
				{
					var encodedUsernamePassword = authHeader
						.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries) [1]
						?.Trim();

					var decodedUsernamePassword = Encoding.UTF8.GetString(
						Convert.FromBase64String(encodedUsernamePassword));

					var username = decodedUsernamePassword.Split(':', 2) [0];
					var password = decodedUsernamePassword.Split(':', 2) [1];

					if (IsAuthorized(username, password))
					{
						await next.Invoke(context);
						return;
					}
				}

				context.Response.Headers["WWW-Authenticate"] = "Basic";
				context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
			}
			else
			{
				await next.Invoke(context);
			}
		}

		public bool IsAuthorized(string username, string password) =>
			username.Equals(options.Username, StringComparison.InvariantCultureIgnoreCase)
			&& password == options.Password;
	}
}