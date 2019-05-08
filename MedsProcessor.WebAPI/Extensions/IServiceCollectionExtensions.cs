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
using MedsProcessor.WebAPI.Utils;
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
		public static IServiceCollection ConfigureIpRequestThrottling(
			this IServiceCollection services,
			IConfiguration config)
		{
			services.AddHttpContextAccessor();

			//load general configuration from appsettings.json
			services.Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"));

			// inject counter and rules stores
			services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
			services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

			// configuration (resolvers, counter key builders)
			//services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

			return services;
		}

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

		public static IServiceCollection ConfigureMvc(this IServiceCollection services)
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

		public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
		{
			services.AddSingleton(
				s => new AppPathsInfo(s.GetService<IHostingEnvironment>().ContentRootPath));

			services.AddSingleton<HzzoHtmlScraper>();
			services.AddSingleton<HzzoExcelDownloader>();
			services.AddSingleton<HzzoExcelParser>();
			services.AddSingleton<HzzoDataProcessor>();
			services.AddSingleton<HzzoData>();

			services.AddScoped<IJwtAuthService, JwtAuthService>();

			return services;
		}

		public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration config)
		{
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
						ValidateIssuerSigningKey = true,
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