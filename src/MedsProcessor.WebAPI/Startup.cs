using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MedsProcessor.Common.Models;
using MedsProcessor.Downloader;
using MedsProcessor.Parser;
using MedsProcessor.Scraper;
using MedsProcessor.WebAPI.Core;
using MedsProcessor.WebAPI.Extensions;
using MedsProcessor.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace MedsProcessor.WebAPI
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddOptions();
      services.AddHttpClient();
      services.AddAngleSharp();

      services.AddSingleton(
        s => new AppPathsInfo(s.GetService<IHostingEnvironment>().ContentRootPath));

      services.AddSingleton<HzzoHtmlScraper>();
      services.AddSingleton<HzzoExcelDownloader>();
      services.AddSingleton<HzzoExcelParser>();
      services.AddSingleton<HzzoData>();
      services.AddSingleton<HzzoDataProcessor>();

      services.AddMvc().AddJsonOptions(opts =>
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
      }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      services.AddApiVersioning(opts =>
      {
        opts.DefaultApiVersion = new ApiVersion(1, 0);
        opts.AssumeDefaultVersionWhenUnspecified = true;
        opts.ReportApiVersions = true;
      });

      services.AddScoped<IJwtAuthService, JwtAuthService>();

      var tokenOptsConfig = Configuration.GetSection(nameof(AuthTokenOptions).Replace("Options", ""));
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
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }

      // Authorize access to the Swagger documentation site for non local requests
      app.UseBasicAuthentication(opts =>
      {
        opts.AuthorizeLocalRequest = false;
        opts.AuthorizeRoutes("/swagger");
        opts.Username = "admin";
        opts.Password = "admin";
      });

      // Enable middleware to serve generated Swagger as a JSON endpoint.
      app.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1-0/swagger.json", "HZZO meds-processor v1.0"));

      app.UseHttpsRedirection();
      app.UseAuthentication();

      // Expose the API for outer domain requests
      app.UseCors(opts =>
        opts.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET", "POST", "OPTIONS"));

      app.UseMvc();
    }
  }
}