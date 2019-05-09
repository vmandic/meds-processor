namespace MedsProcessor.WebAPI.Models
{
	public class AuthTokenResponse
	{
		public AuthTokenResponse() { }
		public AuthTokenResponse(string token)
		{
			this.AccessToken = token;
		}
		public string AccessToken { get; }
	}
}