using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using MedsProcessor.Common.Models;
using static MedsProcessor.Common.Constants;

namespace MedsProcessor.Scraper
{
    public class HzzoHtmlScraper
	{
		static readonly DateTime filterDtParsable2013 = new DateTime(2013, 6, 13);
		readonly IBrowsingContext _browsingContext;

		public HzzoHtmlScraper(IBrowsingContext browsingContext)
		{
			this._browsingContext = browsingContext;
		}

		public async Task<ISet<HzzoMedsDownloadDto>> Run()
		{
			var htmlDocs = await DownloadHtmlDocuments();
			var parsedDocs = ParseHtmlDocuments(htmlDocs);

			return parsedDocs;
		}

		Task<IDocument[]> DownloadHtmlDocuments() =>
			Task.WhenAll(
				_browsingContext.OpenAsync(CURRENT_LISTS_URL),
				_browsingContext.OpenAsync(ARCHIVE_LISTS_URL)
			);

		ISet<HzzoMedsDownloadDto> ParseHtmlDocuments(IDocument[] docs) =>
			docs.Aggregate(
				new HashSet<HzzoMedsDownloadDto>(),
				(docList, doc) => new HashSet<HzzoMedsDownloadDto>(docList.Concat(ParseHtmlDocument(doc)))
			);

		static ISet<HzzoMedsDownloadDto> ParseMedsLiElements(IEnumerable<IElement> elems) =>
			elems.Aggregate(new HashSet<HzzoMedsDownloadDto>(), (medsList, li) =>
			{
				var href = li.QuerySelector("a").GetAttribute("href");

				// NOTE: this domain is not available, links don't work :-(
				if (!href.Contains("cdn.hzzo.hr"))
				{
					var downloadDto = new HzzoMedsDownloadDto(
						href,
						li.TextContent.TrimEnd().Split(' ').LastOrDefault(),
						DOWNLOAD_DIR
					);

					// NOTE: that's it folks, docs from 2013 and older are messed up
					// and can't be approached with this generic parser in this app
					// A more sophisticated parser (more if/else loops...) would be needed
					if (downloadDto.ValidFrom > filterDtParsable2013)
						medsList.Add(downloadDto);
				}

				return medsList;
			});

		static ISet<HzzoMedsDownloadDto> ParseHtmlDocument(IDocument doc) =>
			ParseMedsLiElements(SelectLiElements(doc));

		static IEnumerable<IElement> SelectLiElements(IDocument doc) =>
			doc.QuerySelectorAll("section#main > ul li").Where(_predicateForListLiMeds);

		static Func<IElement, bool> _predicateForListLiMeds =
			x =>
			// primary list:
			x.TextContent.ToLowerInvariant().Contains("osnovna lista lijekova") ||
			// supplementary list:
			x.TextContent.ToLowerInvariant().Contains("dopunska lista lijekova");
	}
}