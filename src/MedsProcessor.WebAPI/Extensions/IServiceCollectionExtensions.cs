using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using MedsProcessor.Common.Models;
using MedsProcessor.Downloader;
using MedsProcessor.Parser;
using MedsProcessor.Scraper;
using MedsProcessor.WebAPI.Core;
using MedsProcessor.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace MedsProcessor.WebAPI.Extensions
{
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Configures the API versioning services with the default 1.0 API version to be used when a version is not specified.
		/// Enables reporting avialable and deprecated API versions through HTTP response headers.
		/// </summary>
		public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services) =>
			services.AddApiVersioning(opts =>
			{
				opts.DefaultApiVersion = new ApiVersion(1, 0);
				opts.AssumeDefaultVersionWhenUnspecified = true;
				opts.ReportApiVersions = true;
			});

		/// <summary>
		/// Adds HTTP web request throttling services (a.k.a. rate limiting) via 'AspNetCoreRateLimit' library based on IP limiting.
		/// </summary>
		/// <remarks>ref: https://github.com/stefanprodan/AspNetCoreRateLimit/wiki</remarks>
		public static IServiceCollection ConfigureHttpRequestThrottlingByIp(
			this IServiceCollection services,
			IConfiguration config)
		{
			services.AddHttpContextAccessor();
			services.Configure<IpRateLimitOptions>(config.GetSection("HttpRequestRateLimit"));

			// inject counter and rules stores
			services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
			services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

			return services;
		}

		/// <summary>
		/// Adds Open API (Swagger) services for documenting the Web API with v1 version.
		/// Adds msbuild documentation into the swagger to provide additional information about the available endpoints.
		/// Adds a 'security definition' for swagger allowing the SwaggerUI library to display an Authorize button for enabling JWT authentication.
		/// </summary>
		public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(opts =>
			{
				opts.SwaggerDoc(
					"v1-0",
					new Info
					{
						Title = "HZZO meds-processor v1.0",
						Version = "1.0"
					});

				// Setup swagger to use msbuild documentation:
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

				opts.IncludeXmlComments(xmlPath);

				opts.AddSecurityDefinition("Bearer", new ApiKeyScheme
				{
					In = "header",
						Description = "Please insert JWT with Bearer into field",
						Name = "Authorization",
						Type = "apiKey"
				});

				opts.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
				{ { "Bearer", new string[] { } }
				});
			});

			return services;
		}

		/// <summary>
		/// Configures MVC services by enforcing a strict check for HTTP request's 'Accept' (a.k.a. strict content negotiation) header.
		/// Configures the HTTP pipeline to return a status code 406 when an invalid formatter is specified through a HTTP header.
		/// Configures the default 'Newtonsoft.Json' serializer to use <see cref="SnakeCaseNamingStrategy"/> for serializing and parsing JSON properties and changes the default and null value handling to be ignored. The default date value is formated as 'yyyy-MM-dd'.
		/// </summary>
		public static IServiceCollection ConfigureMvcAndJsonSerializer(this IServiceCollection services)
		{
			services.AddMvc(opts =>
				{
					// Make the API strict in content negotiation:
					opts.RespectBrowserAcceptHeader = true;
					opts.ReturnHttpNotAcceptable = true;
				})
				.AddJsonOptions(opts =>
				{
					opts.SerializerSettings.ContractResolver = new DefaultContractResolver
					{
						NamingStrategy = new SnakeCaseNamingStrategy()
					};
					opts.SerializerSettings.Formatting = Formatting.Indented;
					opts.SerializerSettings.Converters.Add(new StringEnumConverter());
					opts.SerializerSettings.DateFormatString = "yyyy-MM-dd";
					opts.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
					opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			return services;
		}

		public static IServiceCollection ConfigureCoreDependencies(this IServiceCollection services)
		{
			services.AddSingleton(
				s => new AppPathsInfo(s.GetService<IHostingEnvironment>().ContentRootPath));

			services.AddSingleton<HzzoHtmlScraper>();
			services.AddSingleton<HzzoExcelDownloader>();
			services.AddSingleton<HzzoExcelParser>();
			services.AddSingleton<HzzoDataProcessor>();
			services.AddSingleton<HzzoData>();

			return services;
		}

		/// <summary>
		/// Configures the HTTP pipeline to use JWT authentication as the default auth scheme.
		/// Configures the JWT options by reading configuration options for <see cref="AuthTokenOptions" />.
		/// </summary>
		public static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration config)
		{
			services.AddScoped<IJwtAuthService, JwtAuthService>();

			var tokenOptsConfig = config.GetSection(nameof(AuthTokenOptions).Replace("Options", ""));
			var tokenOpts = tokenOptsConfig.Get<AuthTokenOptions>();
			services.Configure<AuthTokenOptions>(tokenOptsConfig);

			services.AddAuthentication(opts =>
			{
				opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(opts =>
			{
				opts.RequireHttpsMetadata = true;
				opts.SaveToken = true;
				opts.ClaimsIssuer = tokenOpts.Issuer;
				opts.TokenValidationParameters = new TokenValidationParameters
				{
					RequireSignedTokens = true,
					RequireExpirationTime = true,
					IssuerSigningKey = tokenOpts.Key,
					ValidIssuer = tokenOpts.Issuer,
					ValidAudience = tokenOpts.Audience,
					ValidateIssuer = true,
					ValidateAudience = false,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true
				};

				opts.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
						{
							context.Response.Headers.Add("Token-Expired", "true");
						}
						return Task.CompletedTask;
					}
				};
			});

			return services;
		}
	}
}