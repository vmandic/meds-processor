using System;
using System.ComponentModel;
using MedsProcessor.Common;
using Newtonsoft.Json;

namespace MedsProcessor.Common.Models
{
	public class HzzoMedsImportDto
	{
		public int RowId { get; set; }
		public DrugListType ListType { get; set; }
		public DateTime ValidFrom { get; set; }

		public string AtkCode { get; set; }
		public DrugApplicationTypeLimitation ApplicationTypeLimitation { get; set; }

		[DefaultValue("")]
		public string GenericName { get; set; }

		[DefaultValue("")]
		public string UnitOfDistribution { get; set; }
		public decimal? UnitOfDistributionPriceWithoutPDV { get; set; }
		public decimal? UnitOfDistributionPriceWithPDV { get; set; }
		public DrugApplicationType ApplicationType { get; set; }

		[DefaultValue("")]
		public string Manufacturer { get; set; }

		[DefaultValue("")]
		public string ApprovedBy { get; set; }

		[DefaultValue("")]
		public string RegisteredName { get; set; }

		[DefaultValue("")]
		public string OriginalPackagingDescription { get; set; }

		// PRIMARY AND SUPPLEMENTARY LIST ORIG. PACKAGING PRICES:

		public decimal? OriginalPackagingSingleUnitPriceWithoutPdv { get; set; }
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

		[DefaultValue("")]
		public string IndicationsCode { get; set; }

		[DefaultValue("")]
		public string DirectionsCode { get; set; }

		[DefaultValue("")]
		public string DrugGroup { get; set; }

		[DefaultValue("")]
		public string DrugGroupCode { get; set; }

		[DefaultValue("")]
		public string DrugSubgroup { get; set; }

		[DefaultValue("")]
		public string DrugSubgroupCode { get; set; }
	}
}