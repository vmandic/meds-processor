using System;
using System.Net;
using Newtonsoft.Json;

namespace MedsProcessor.WebAPI.Models
{
	public class ApiMessageResponse : ApiHttpResponse
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string Message { get; }

		public ApiMessageResponse(int statusCode, string message) : base(statusCode)
		{
			this.Message = message;
		}
	}
}