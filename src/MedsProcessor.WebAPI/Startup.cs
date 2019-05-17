using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedsProcessor.Common.Models;
using MedsProcessor.Downloader;
using MedsProcessor.Parser;
using MedsProcessor.Scraper;
using MedsProcessor.WebAPI.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
}