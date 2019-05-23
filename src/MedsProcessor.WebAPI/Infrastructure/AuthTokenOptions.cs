using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MedsProcessor.WebAPI.Infrastructure
{
	public class AuthTokenOptions
	{
		public string Secret { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public DateTime NotBefore => DateTime.UtcNow;
		DateTime IssuedAt => DateTime.UtcNow;
		public long IssuedAtAsUnixEpoch => ToUnixEpochDate(IssuedAt);
		public DateTime Expiration => IssuedAt.Add(ValidFor);
		public TimeSpan ValidFor => TimeSpan.FromSeconds(ValidForSeconds);
		public int ValidForSeconds { get; set; }
		public SymmetricSecurityKey Key => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
		public SigningCredentials SigningCredentials =>
			new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

		private static long ToUnixEpochDate(DateTime date) =>
			(long) Math.Round(
				(date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
				.TotalSeconds);
	}
}