using System;
using System.Net;

namespace MedsProcessor.WebAPI.Models
{
	/// <summary>
	/// For the sake of the tutorial, this class will wrap over HTTP header data and provide the HTTP status code and its description.
	/// </summary>
	/// <remarks>Do not consider using this class at all in production environments. Your base class can instead be <see cref="ApiMessageResponse" /> which can include an additional <c>IsError</c> boolean property indicating if the HTTP request processing errored.</remarks>
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