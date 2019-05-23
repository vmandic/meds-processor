using AspNetCoreRateLimit;
using MedsProcessor.Scraper;
using MedsProcessor.WebAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
      services.AddMemoryCache();
      services.AddHttpContextAccessor();
      services.AddOptions();
      services.AddHttpClient();
      services.AddAngleSharp();

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
      app.UseIpRateLimiting();

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