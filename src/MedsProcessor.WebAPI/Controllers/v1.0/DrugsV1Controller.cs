using System;
using System.Collections.Generic;
using System.Linq;
using MedsProcessor.Common;
using MedsProcessor.Common.Models;
using MedsProcessor.WebAPI.Core;
using MedsProcessor.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers.v1_0
{
  public class DrugsV1Controller : ApiV1ControllerBase
  {
    private readonly IEnumerable<HzzoMedsImportDto> _allMeds;

    public DrugsV1Controller(HzzoData data)
    {
      this._allMeds = data.Set.SelectMany(x => x.MedsList);
    }

    /// <summary>
    /// Gets a list of drugs by specifying the type and year(s).
    /// </summary>
    /// <param name="type">Drug list type: All, Primary or Supplemnetary</param>
    /// <param name="years">A year or years comma delimited list to lookup the desired drugs.</param>
    /// <param name="page">Page number of items being retrieved.</param>
    /// <param name="size">Number of items per page to retrieve.</param>
    /// <returns>Returns a paged or unpaged JSON list of drugs filtered by query parameters.</returns>
    [HttpGet("drugs/list/{type}/{years:regex(^(\\d{{1,}},?)*$)?}")]
    [ProducesResponseType(typeof(ApiDataResponse<IEnumerable<HzzoMedsImportDto>>), 200)]
    [ProducesResponseType(typeof(ApiPagedDataResponse<IEnumerable<HzzoMedsImportDto>>), 200)]
    public ActionResult GetDumpJson(
      DrugListTypeFilter type,
      string years = null,
      int? page = null,
      int? size = null)
    {
      var result =
        FilterByYears(years,
          FilterByType(type, _allMeds))
        .ToList();

      return ApiResponse.TryForPagedData(result, page, size);
    }

    /// <summary>
    /// Searches for all drugs containing the search query parameter in any of the following parameters:
    /// GenericName, RegisteredName, Manufacturer, ApprovedBy, DrugGroup, DrugSubgroup and OriginalPackagingDescription.
    /// </summary>
    /// <param name="searchQuery">The drug search query of length from 1 to 50 character to lookup the whole list of drugs.</param>
    /// <param name="page">Page number of items being retrieved.</param>
    /// <param name="size">Number of items per page to retrieve.</param>
    /// <returns>Returns a paged or unpaged JSON list containing the found drugs mateched by the provided search query parameter.</returns>
    [HttpGet("drugs/search/{searchQuery:length(1,50)}")]
    [ProducesResponseType(typeof(ApiDataResponse<IEnumerable<HzzoMedsImportDto>>), 200)]
    [ProducesResponseType(typeof(ApiPagedDataResponse<IEnumerable<HzzoMedsImportDto>>), 200)]
    public ActionResult GetSearchForDrug(
      string searchQuery,
      int? page = null,
      int? size = null)
    {
      bool ContainsSearchForProp(string str) =>
        str != null && str.Contains(searchQuery, StringComparison.OrdinalIgnoreCase);

      bool ContainsSearchForAnyOf(params string[] args) =>
        args.AsEnumerable().Any(ContainsSearchForProp);

      var result = _allMeds.Where(x =>
          ContainsSearchForAnyOf(
            x.GenericName,
            x.RegisteredName,
            x.Manufacturer,
            x.ApprovedBy,
            x.DrugGroup,
            x.DrugSubgroup,
            x.OriginalPackagingDescription))
        .ToList();

      return ApiResponse.TryForPagedData(result, page, size);
    }

    /// <summary>
    /// Searches for all drugs eaxctly matching the drug ATK code parameter.
    /// </summary>
    /// <param name="atkCode">The unique ATK code of a drug or medicine usually in lenght of 10 to 12 charaters including a blank space on the third from the end index of the string. Minimum length of the parameter is 4 and maximum 12 characters.</param>
    /// <returns>Returns a JSON list containing the found drugs mateched by the provided ATK code query parameter.</returns>
    [HttpGet("drugs/for/atk/{atkCode:length(4,12)}")]
    public ActionResult<ApiDataResponse<IEnumerable<HzzoMedsImportDto>>> GetListByAtkCode(
      string atkCode)
    {
      // Ensure that the atk code string ends with a whitespace followed by three characters
      // e.g. 'A0B1C2 123', 'DBCDC2 411'...
      if (atkCode.Reverse().ToArray() [3] != ' ')
      {
        atkCode = atkCode.Insert(atkCode.Length - 3, " ");
      }

      var result = _allMeds
        .Where(x => x.AtkCode.Equals(atkCode, StringComparison.OrdinalIgnoreCase))
        .OrderBy(x => x.ValidFrom)
        .ToList();

      return ApiResponse.ForData(
        result,
        message: $"Total results for atk code '{atkCode}': {result.Count()}");
    }

    /// <summary>
    /// Searches for all drugs produced by a manufacturer.
    /// The lookup works by checking if the manufacturer name contains the provided manufacturer parameter.
    /// </summary>
    /// <param name="manufacturer">
    /// The drug manufacturer.
    /// Can ba a part of the full name of a manufacturer.
    /// </param>
    /// <returns>
    /// Returns a JSON list containing the found drugs mateched by the provided manufacturer query parameter.
    /// </returns>
    [HttpGet("drugs/by-matching/manufacturer/{manufacturer:length(1,50)}")]
    public ActionResult<ApiDataResponse<IEnumerable<HzzoMedsImportDto>>> GetListByManufacturer(
      string manufacturer)
    {
      manufacturer = manufacturer.Trim();

      var result = _allMeds
        .Where(x => x.Manufacturer.Contains(manufacturer, StringComparison.OrdinalIgnoreCase))
        .OrderBy(x => x.ValidFrom)
        .ToList();

      return ApiResponse.ForData(
        result,
        message: $"Total results where manufacturer contains '{manufacturer}': {result.Count()}");
    }

    private static IEnumerable<HzzoMedsImportDto> FilterByYears(
      string years,
      IEnumerable<HzzoMedsImportDto> result)
    {
      if (!string.IsNullOrEmpty(years))
      {
        var filter = years
          .Split(",", StringSplitOptions.RemoveEmptyEntries)
          .Select(int.Parse)
          .ToList();

        result = result.Where(x => filter.Contains(x.ValidFrom.Year));
      }

      return result;
    }

    private static IEnumerable<HzzoMedsImportDto> FilterByType(
      DrugListTypeFilter type,
      IEnumerable<HzzoMedsImportDto> result)
    {
      if (type != DrugListTypeFilter.All)
      {
        var targetType = (DrugListType) (int) type;
        result = result.Where(x => x.ListType == targetType);
      }

      return result;
    }
  }
}