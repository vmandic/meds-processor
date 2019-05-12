using System;

namespace MedsProcessor.Common.Models
{
	public class HzzoMedsImportDto
	{
			public int RowId { get; set; }
			public DrugListType ListType { get; set; }
			public DateTime ValidFrom { get; set; }

			public string AtkCode { get; set; }
			public DrugApplicationTypeLimitation ApplicationTypeLimitation { get; set; }

			public string GenericName { get; set; }
			public string UnitOfDistribution { get; set; }
			public decimal? UnitOfDistributionPriceWithoutPDV { get; set; }
			public decimal? UnitOfDistributionPriceWithPDV { get; set; }
			public DrugApplicationType ApplicationType { get; set; }
			public string Manufacturer { get; set; }
			public string ApprovedBy { get; set; }
			public string RegisteredName { get; set; }
			public string OriginalPackagingDescription { get; set; }

			// PRIMARY AND SUPPLEMENTARY LIST ORIG. PACKAGING PRICES:

			public decimal? OriginalPackagingSingleUnitPriceWithoutPdv{ get; set; }
			public decimal? OriginalPackagingSingleUnitPriceWithPdv { get; set; }
			public decimal? OriginalPackagingPriceWithoutPdv { get; set; }
			public decimal? OriginalPackagingPriceWithPdv { get; set; }

			// SUPPLEMENTARY LIST PRICES:

			// SINGLE UNIT SOLD WITH ORIG. PACKAGING
			public decimal? OriginalPackagingSingleUnitPricePaidByHzzoWithoutPdv { get; set; }
			public decimal? OriginalPackagingSingleUnitPricePaidByHzzoWithPdv { get; set; }
			public decimal? OriginalPackagingSingleUnitPriceExtraChargeWithoutPdv { get; set; }
			public decimal? OriginalPackagingSingleUnitPriceExtraChargeWithPdv { get; set; }

			// SINGLE ORIGINAL PACKAGING UNIT
			public decimal? OriginalPackagingPricePaidByHzzoWithoutPdv { get; set; }
			public decimal? OriginalPackagingPricePaidByHzzoWithPdv { get; set; }
			public decimal? OriginalPackagingPriceExtraChargeWithoutPdv { get; set; }
			public decimal? OriginalPackagingPriceExtraChargeWithPdv { get; set; }


			public DrugPrescriptionType PrescriptionType { get; set; }
			public string IndicationsCode { get; set; }
			public string DirectionsCode { get; set; }
			public string DrugGroup { get; set; }
			public string DrugGroupCode { get; set; }
			public string DrugSubgroup { get; set; }
			public string DrugSubgroupCode { get; set; }
		}
}