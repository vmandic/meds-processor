using AspNetCoreRateLimit;
using MedsProcessor.Scraper;
using MedsProcessor.WebAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
			services.AddMemoryCache();
			services.AddHttpContextAccessor();
			services.AddHttpClient();
			services.AddAngleSharp();
			services.AddResponseCompression(opts => opts.EnableForHttps = true);
			services.AddHealthChecks();

			services.ConfigureApiVersioning();
			services.ConfigureHttpRequestThrottlingByIp(Configuration);
			services.ConfigureCoreDependencies();
			services.ConfigureJwtAuthentication(Configuration);
			services.ConfigureMvcAndJsonSerializer();
			services.ConfigureSwagger();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsProduction())
				app.UseHsts();

			app.UseHttpsRedirection();

			// Expose the API for outer domain requests
			app.UseCors(opts =>
				opts.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET", "POST", "OPTIONS"));

			app.UseIpRateLimiting();

			// Handles exceptions and generates a custom response body
			app.UseExceptionHandler("/api/error");

			app.UseAuthentication();

			// Authorize access to the Swagger documentation site for non local requests
			app.UseBasicAuthentication(opts =>
			{
				opts.AuthorizeLocalRequest = false;
				opts.AuthorizeRoutes("/swagger");
				opts.Username = "admin";
				opts.Password = "admin";
			});

			app.UseResponseCompression();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
				c.SwaggerEndpoint("/swagger/v1-0/swagger.json", "HZZO meds-processor v1.0"));

			app.UseHealthChecks("/health");

			app.UseMvc();
		}
	}
}