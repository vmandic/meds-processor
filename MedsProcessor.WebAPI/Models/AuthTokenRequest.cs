using System.ComponentModel.DataAnnotations;

namespace MedsProcessor.WebAPI.Models
{
	public class AuthTokenRequest
	{
		[Required]
		public string ClientId { get; set; }
	}
}