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

			services.AddSingleton(
				s => new AppPathsInfo(s.GetService<IHostingEnvironment>().ContentRootPath));

			services.AddSingleton<HzzoHtmlScraper>();
			services.AddSingleton<HzzoExcelDownloader>();
			services.AddSingleton<HzzoExcelParser>();
			services.AddSingleton<HzzoDataProcessor>();
			services.AddSingleton<HzzoData>();

			services.AddMvc()
				.AddJsonOptions(opts =>
				{
					opts.SerializerSettings.ContractResolver = new DefaultContractResolver
					{
						NamingStrategy = new SnakeCaseNamingStrategy()
					};
					opts.SerializerSettings.Converters.Add(new StringEnumConverter());
					opts.SerializerSettings.DateFormatString = "yyyy-MM-dd";
					opts.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
					opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (!env.IsDevelopment())
			{
				app.UseHsts();
			}

			// Handles exceptions and generates a custom response body
			app.UseExceptionHandler("/error/500");

			// Handles non-success status codes with empty body
			app.UseStatusCodePagesWithReExecute("/error/{0}");

			app.UseHttpsRedirection();
			app.UseResponseCompression();
			app.UseMvc();
		}
	}
}