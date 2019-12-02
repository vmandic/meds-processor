using Newtonsoft.Json;

namespace MedsProcessor.WebAPI.Models
{
	public class ApiMessageResponse
	{
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string Message { get; }

		public ApiMessageResponse(string message) : base()
		{
			this.Message = message;
		}
	}
}