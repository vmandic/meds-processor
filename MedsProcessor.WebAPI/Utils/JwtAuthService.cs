using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedsProcessor.WebAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MedsProcessor.WebAPI.Utils
{
	public class JwtAuthService
	{
		private const string DEFAULT_CLIENT_ID = "default";
		private readonly AuthTokenOptions _tokenOpts;
		public JwtAuthService(IOptions<AuthTokenOptions> tokenOpts)
		{
			this._tokenOpts = tokenOpts.Value;
		}

		public(bool authenticatedSuccessfully, string token) IssueToken(AuthTokenRequest tokenRequest)
		{
			if (tokenRequest == null)
				throw new ArgumentNullException(nameof(tokenRequest));

			// Here you can implement username/password verification instead
			if (tokenRequest.ClientId != DEFAULT_CLIENT_ID)
				return (false, null);

			var jwtToken = new JwtSecurityToken(
				_tokenOpts.Issuer,
				_tokenOpts.Audience,
				new []
				{
					new Claim(JwtRegisteredClaimNames.Sub, tokenRequest.ClientId),
						new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						new Claim(JwtRegisteredClaimNames.Iat, _tokenOpts.IssuedAtAsUnixEpoch.ToString(), ClaimValueTypes.Integer64),
				},
				_tokenOpts.NotBefore,
				_tokenOpts.Expiration,
				_tokenOpts.SigningCredentials
			);

			return (true, new JwtSecurityTokenHandler().WriteToken(jwtToken));
		}
	}
}