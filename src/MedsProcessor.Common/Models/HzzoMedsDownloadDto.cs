using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MedsProcessor.Common.Models
{
  public class HzzoMedsDownloadDto
  {
    private readonly string _rootLocation;

    public HzzoMedsDownloadDto(string href, string validFrom, string rootLocation)
    {
      this.Href = href;
      this.ValidFrom = DateTime.Parse(validFrom);
      this._rootLocation = rootLocation;
    }

    [JsonIgnore]
    public string FilePath =>
      Path.Combine(_rootLocation, FileName);

    [JsonIgnore]
    public bool IsDownloaded =>
      File.Exists(FilePath);

    [JsonIgnore]
    public string FileName =>
      ValidFrom.ToString("yyyy-MM-dd_") +
      (Href.Split('/').LastOrDefault() ?? Href.Replace("/", "_").Replace(":", "_")).TrimEnd();

    public ISet<HzzoMedsImportDto> MedsList { get; } = new HashSet<HzzoMedsImportDto>();
    public string Href { get; set; }
    public DateTime ValidFrom { get; private set; }

    [JsonIgnore]
    public Task<Stream> DocumentStream { get; set; }

    [JsonIgnore]
    public bool IsDataParsed { get; private set; }

    public void MarkAsParsed() =>
      IsDataParsed = true;
  }
}