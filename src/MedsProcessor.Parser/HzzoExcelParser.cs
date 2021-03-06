using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedsProcessor.Common;
using MedsProcessor.Common.Extensions;
using MedsProcessor.Common.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.Extensions.Logging;

namespace MedsProcessor.Parser
{
	public class HzzoExcelParser
	{
		private readonly ILogger<HzzoExcelParser> _logger;
		private int _medsCount;
		private int _parsedCount;

		public HzzoExcelParser(ILogger<HzzoExcelParser> logger)
		{
			_logger = logger;
		}

		public ISet<HzzoMedsDownloadDto> Run(ISet<HzzoMedsDownloadDto> meds)
		{
			// HACK: due to a bug with the parser introduced in 2019, we are skipping year 2019
			_medsCount = meds.Count(x => x.ValidFrom.Year < 2019);
			_parsedCount = 0;

			// NOTE: we can do this below in parallel but this way it is more stable due to file
			// lock possibility with the file system and dotnet core process

			ParsePrimaryListsUpTo2014_01(meds);
			ParseSupplementaryListsUpTo2014_01(meds);

			ParsePrimaryListsStartingWith2014_02(meds);
			ParseSupplementaryListsStartingWith2014_02(meds);

			_logger.LogInformation("Parser complete!");
			return meds;
		}

		ISheet OpenWorkbookSheetWithNpoi(
			FileStream stream,
			HzzoMedsDownloadDto med,
			HzzoMedsDownloadDto latestMed)
		{
			ISheet drugListSheet = null;

			try
			{
				if (med.FileName.ToLowerInvariant().EndsWith(".xls"))
				{
					var hssfWorkbook = new HSSFWorkbook(stream);
					drugListSheet = hssfWorkbook.GetSheetAt(0);
				}
				else
				{
					var xssfWorkbook = new XSSFWorkbook(stream);
					drugListSheet = xssfWorkbook.GetSheetAt(0);
				}
			}
			catch(Exception ex)
			{
				latestMed.Href += " - WORKSHEET COULD NOT BE PARSED";
				_logger.LogError(ex, latestMed.Href);
			}

			return drugListSheet;
		}

		void ParseHzzoExcelDocuments(IEnumerable<HzzoMedsDownloadDto> filteredMeds, DrugListType listType, bool isListStartingWith2014)
		{
			HzzoMedsDownloadDto latestMed = null;
			int latestRow = 0;
			int latestCol = 0;

			try
			{
				// HACK: due to a bug with the parser introduced in 2019, we are skipping year 2019
				foreach (var med in filteredMeds.Where(x => x.ValidFrom.Year < 2019))
				{
					latestMed = med;
					using(var stream = File.Open(med.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						var drugListSheet = OpenWorkbookSheetWithNpoi(stream, med, latestMed);
						if (drugListSheet == null) return;

						var totalRows = drugListSheet.LastRowNum;
						int rowIndex = 1; // skips header row
						int colIndex = 0;

						int incColNumber() =>
							latestCol = colIndex++;

						string GetNextString() =>
							drugListSheet.GetRow(rowIndex).GetCell(incColNumber()).ToString();

						(bool, string) TryGetNextString()
						{
							var row = drugListSheet.GetRow(rowIndex);
							var cIndex = incColNumber();

							if (row == null || row.GetCell(cIndex) == null)
								return (false, null);

							return (true, drugListSheet.GetRow(rowIndex).GetCell(cIndex).ToString());
						}

						decimal? GetNextDecimal() =>
							decimal.TryParse(
								drugListSheet.GetRow(rowIndex).GetCell(incColNumber()).ToString(),
								out decimal dec)
									? dec
									: new decimal?();

						string GetEnumStrVal() =>
							drugListSheet.GetRow(rowIndex).GetCell(incColNumber())
							.ToString().Replace(@"\", string.Empty).Replace("/", string.Empty);

						DrugApplicationType ParseNextDrugApplicationType()
						{
							var strVal = GetEnumStrVal();

							return string.IsNullOrWhiteSpace(strVal)
									? DrugApplicationType.Undefined
									: EnumExtensions.Parse<DrugApplicationTypeShort, DrugApplicationType>(strVal);
						}

						DrugPrescriptionType ParseNextDrugPrescriptionType()
						{
							var strVal = GetEnumStrVal();

							return string.IsNullOrWhiteSpace(strVal)
									? DrugPrescriptionType.Unprescribed
									: EnumExtensions.Parse<DrugPrescriptionTypeShort, DrugPrescriptionType>(strVal);
						}

						DrugApplicationTypeLimitation ParseNextDrugApplicationTypeLimitation()
						{
							var strVal = GetEnumStrVal();

							return string.IsNullOrWhiteSpace(strVal)
									? DrugApplicationTypeLimitation.Undefined
									: EnumExtensions.Parse<DrugApplicationTypeLimitationShort, DrugApplicationTypeLimitation>(strVal);
						}

						for (; rowIndex <= totalRows; rowIndex++)
						{
							latestCol = colIndex = 0;
							latestRow = rowIndex;

							var (hasRow, atkCode) = TryGetNextString();

							if (!hasRow) continue;

							var importDto = new HzzoMedsImportDto();

							importDto.RowId = rowIndex;
							importDto.ListType = listType;
							importDto.ValidFrom = med.ValidFrom;

							importDto.AtkCode = atkCode;
							importDto.ApplicationTypeLimitation = ParseNextDrugApplicationTypeLimitation();
							importDto.GenericName = GetNextString();
							importDto.UnitOfDistribution = GetNextString();
							importDto.UnitOfDistributionPriceWithoutPDV = GetNextDecimal();
							importDto.UnitOfDistributionPriceWithPDV = GetNextDecimal();
							importDto.ApplicationType = ParseNextDrugApplicationType();
							importDto.ApprovedBy = isListStartingWith2014 ? GetNextString() : null;
							importDto.Manufacturer = GetNextString();
							importDto.RegisteredName = GetNextString();
							importDto.OriginalPackagingDescription = GetNextString();
							importDto.OriginalPackagingSingleUnitPriceWithoutPdv = GetNextDecimal();
							importDto.OriginalPackagingSingleUnitPriceWithPdv = GetNextDecimal();
							importDto.OriginalPackagingPriceWithoutPdv = GetNextDecimal();
							importDto.OriginalPackagingPriceWithPdv = GetNextDecimal();

							// NOTE: supplementary prices
							if (listType == DrugListType.Supplementary)
							{
									importDto.OriginalPackagingSingleUnitPricePaidByHzzoWithoutPdv = GetNextDecimal();
									importDto.OriginalPackagingSingleUnitPricePaidByHzzoWithPdv = GetNextDecimal();
									importDto.OriginalPackagingPricePaidByHzzoWithoutPdv = GetNextDecimal();
									importDto.OriginalPackagingPricePaidByHzzoWithPdv = GetNextDecimal();
									importDto.OriginalPackagingSingleUnitPriceExtraChargeWithoutPdv = GetNextDecimal();
									importDto.OriginalPackagingSingleUnitPriceExtraChargeWithPdv = GetNextDecimal();
									importDto.OriginalPackagingPriceExtraChargeWithoutPdv = GetNextDecimal();
									importDto.OriginalPackagingPriceExtraChargeWithPdv = GetNextDecimal();
							}

							importDto.PrescriptionType = ParseNextDrugPrescriptionType();
							importDto.IndicationsCode = GetNextString();
							importDto.DirectionsCode = GetNextString();
							importDto.DrugGroupCode = GetNextString();
							importDto.DrugGroup = GetNextString();
							importDto.DrugSubgroupCode = GetNextString();
							importDto.DrugSubgroup = GetNextString();

							med.MedsList.Add(importDto);
						}
					}

					med.MarkAsParsed();

					_logger.LogInformation(
						"Parsed document ({parsedCount}/{medsCount}): '{filename}'",
						++_parsedCount,
						_medsCount,
						med.FileName);
				};
			}
			catch (Exception ex)
			{
				var str = new StringBuilder()
					.AppendLine(" latest med: ").Append(latestMed.FileName)
					.AppendLine(" latest row: ").Append(latestRow)
					.AppendLine(" latest col: ").Append(latestCol);

				throw new InvalidOperationException(str.ToString(), ex);
			}
		}

		static readonly DateTime filterDtStartWith2014 = new DateTime(2014, 1, 3);

		void ParseSupplementaryListsUpTo2014_01(ISet<HzzoMedsDownloadDto> meds) =>
			ParseHzzoExcelDocuments(meds.Where(x =>
				x.ValidFrom <= filterDtStartWith2014 &&
				(
					x.FileName.ToLowerInvariant().Contains("dopunska") ||
					x.FileName.ToLowerInvariant().Contains("dll")
				)), DrugListType.Supplementary, false);

		void ParsePrimaryListsUpTo2014_01(ISet<HzzoMedsDownloadDto> meds) =>
			ParseHzzoExcelDocuments(meds.Where(x =>
				x.ValidFrom <= filterDtStartWith2014 &&
				(
					x.FileName.ToLowerInvariant().Contains("osnovna") ||
					x.FileName.ToLowerInvariant().Contains("oll")
				)), DrugListType.Primary, false);

		void ParseSupplementaryListsStartingWith2014_02(ISet<HzzoMedsDownloadDto> meds) =>
			ParseHzzoExcelDocuments(meds.Where(x =>
				x.ValidFrom > filterDtStartWith2014 &&
				(
					x.FileName.ToLowerInvariant().Contains("dopunska") ||
					x.FileName.ToLowerInvariant().Contains("dll")
				)), DrugListType.Supplementary, true);

		void ParsePrimaryListsStartingWith2014_02(ISet<HzzoMedsDownloadDto> meds) =>
			ParseHzzoExcelDocuments(meds.Where(x =>
				x.ValidFrom > filterDtStartWith2014 &&
				(
					x.FileName.ToLowerInvariant().Contains("osnovna") ||
					x.FileName.ToLowerInvariant().Contains("oll")
				)), DrugListType.Primary, true);
	}
}