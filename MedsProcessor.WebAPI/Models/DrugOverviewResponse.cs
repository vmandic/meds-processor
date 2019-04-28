using System;
using System.Collections.Generic;
using MedsProcessor.Common;
using MedsProcessor.Common.Models;

namespace MedsProcessor.WebAPI.Models
{
    public class DrugOverviewResponse
    {
        public IEnumerable<HzzoMedsImportDto> Results { get; set; }
				public int ResultsCount { get; set; }
				public DrugListType ListType { get; set; }
				public string GenericName { get; set; }
				public DateTime FirstValidFrom { get; set; }
				public DateTime LastValidFrom { get; set; }

    }
}