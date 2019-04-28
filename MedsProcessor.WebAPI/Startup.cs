using System;
using System.IO;
using System.Reflection;
using MedsProcessor.Common.Models;
using MedsProcessor.Downloader;
using MedsProcessor.Parser;
using MedsProcessor.Scraper;
using MedsProcessor.WebAPI.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
			services.AddHttpClient();
			services.AddAngleSharp();
			services.AddResponseCompression();
			services.AddApiVersioning();

			services.AddSingleton(
				s => new AppPathsInfo(s.GetService<IHostingEnvironment>().ContentRootPath));

			services.AddSingleton<HzzoHtmlScraper>();
			services.AddSingleton<HzzoExcelDownloader>();
			services.AddSingleton<HzzoExcelParser>();
			services.AddSingleton<HzzoDataProcessor>();
			services.AddSingleton<HzzoData>();

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

			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new Info { Title = "HZZO meds-processor", Version = "v1" });

				// Setup swagger to use msbuild documentation:
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				options.IncludeXmlComments(xmlPath);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (!env.IsDevelopment())
			{
				app.UseHsts();
			}

			// Expose the API for outer domain requests
			app.UseCors();

			// Handles exceptions and generates a custom response body
			app.UseExceptionHandler("/error/500");

			// Handles non-success status codes with empty body
			app.UseStatusCodePagesWithReExecute("/error/{0}");

			app.UseHttpsRedirection();
			app.UseResponseCompression();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "HZZO meds-processor v1");
			});

			app.UseMvc();
		}
	}
}