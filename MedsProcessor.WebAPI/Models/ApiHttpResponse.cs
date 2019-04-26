using System;
using System.Net;

namespace MedsProcessor.WebAPI.Models
{
	public class ApiHttpResponse
	{
		public int StatusCode { get; }

		public string StatusDescription { get; }

		public ApiHttpResponse(int statusCode)
		{
			if (!Enum.IsDefined(typeof(HttpStatusCode), statusCode))
			{
				throw new ArgumentOutOfRangeException(nameof(statusCode), statusCode, "The provided HTTP status code is invalid.");
			}

			this.StatusCode = statusCode;
			this.StatusDescription = ((HttpStatusCode) statusCode).ToString();
		}
	}
}