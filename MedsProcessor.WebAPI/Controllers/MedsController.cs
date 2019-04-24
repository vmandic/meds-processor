using System;
using System.Collections.Generic;
using System.Linq;
using MedsProcessor.Common.Models;
using MedsProcessor.Parser;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
	{
		[ApiController, Route("[controller]")]
		public class MedsController : ControllerBase
		{
			public enum ListType
			{
				All = DrugListType.Undefined,
				Primary = DrugListType.Primary,
				Supplementary = DrugListType.Supplementary
			}

			private readonly HzzoData _data;
			public MedsController(HzzoData data)
			{
				this._data = data;
			}

			[HttpGet("list/{type}/{years:regex(^(\\d{{1,}},?)*$)?}")]
			public ActionResult<IEnumerable<HzzoMedsImportDto>> DumpJson(ListType type, string years = null)
			{
				var result = _data.Set.SelectMany(x => x.MedsList);

				result = FilterByType(type, result);
				result = FilterByYears(years, result);

				return Ok(result);
			}

			private static IEnumerable<HzzoMedsImportDto> FilterByYears(string years, IEnumerable<HzzoMedsImportDto> result)
			{
				if (!string.IsNullOrEmpty(years))
				{
					var filter = years.Split(",").Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
					result = result.Where(x => filter.Contains(x.ValidFrom.Year));
				}

				return result;
			}

			private static IEnumerable<HzzoMedsImportDto> FilterByType(ListType type, IEnumerable<HzzoMedsImportDto> result)
			{
				if (type != ListType.All)
				{
					var targetType = (DrugListType) (int) type;
					result = result.Where(x => x.ListType == targetType);
				}

				return result;
			}

			[HttpGet("search/{searchQuery:length(1,50)}")]
			public ActionResult<IEnumerable<HzzoMedsImportDto>> SearchForDrug(string searchQuery)
			{
				var result = _data.Set.SelectMany(x => x.MedsList);

				bool ContainsSearchForProp(string str) =>
					str.Contains(searchQuery, StringComparison.OrdinalIgnoreCase);

				bool ContainsSearchForAnyOf(params string[] args) =>
					args.AsEnumerable().Any(ContainsSearchForProp);

				result = result.Where(x =>
					ContainsSearchForAnyOf(
						x.GenericName,
						x.RegisteredName,
						x.Manufacturer,
						x.ApprovedBy,
						x.DrugGroup,
						x.DrugSubgroup,
						x.OriginalPackagingDescription));

					return Ok(result);
				}
			}
		}