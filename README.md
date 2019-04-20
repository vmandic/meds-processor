# The build a `meds-processor` guide to C# and .NET Core

**This is the perfect place to start learning C# and .NET Core by building something real with looks and feels to it - a drug list data scraper and REST web API.**

## About / Summary

Learn to build a web scraper, downloader & Excel parser by digging through some hiddious spreadsheet data of Croatia's Health Insurance Fund and its primary and supplementary list of drugs and all that by using only C# and .NET Core (on any modern computer platform)!

## The repository is composed of 4 parts

The repository is composed of four parts, i.e. four branches where each of them has their own blog post article. You can browse through branches here in Github on the branch selection dropdown.

### Branches / parts / articles

- **part/1** ([Practical .NET Core  🚀— write a web scraper, downloader & Excel parser. Part 1: Scraper](https://medium.com/@vekzdran/practical-net-core-write-a-web-scraper-downloader-excel-parser-part-1-4-ece43e0af898))
  - The thrilling one. Bootstraps you to the C# and .NET world. Sets you up with the whole project. Introduces you to .NET Core, the dotnet CLI where you will create a web scraper app using the `AngleSharp` library to fetch some remote HTML pages and extract some links.
<br/>
- **part/2** ([Practical .NET Core 🚀— write a scraper, fetcher & xls(x) parser. Part 2: Downloader](https://medium.com/@vekzdran/practical-net-core-write-a-scraper-fetcher-xls-x-parser-part-2-parallel-downloading-fc4d21f21417))
  - The most exciting one. Continues on part/1 where now you will download the documents to which the former links lead to. The documents are .xls(x) spreadsheets that you will save to your disk drive. You will learn how to utilized the `Task Parallel Library` and process async Tasks in .NET Core.
<br/>
- **part/3** ([Practical .NET Core 🚀— write a scraper, fetcher & .xls(x) parser. Part 3: Parser](https://medium.com/@vekzdran/practical-net-core-build-a-scraper-fetcher-xls-x-parser-part-3-cross-platform-parser-657822ea8471))
  - The longest and thughest. Continues on part/2 where you have downloaded spreadsheet docs that need parsing into C# models. You will learn to use the `NPOI` spreadsheets parsing library to extract relevant data for your C# model classes. Upon finishing you will have a single dataset of transformed and organized data.
<br/>
- **part/4** (A work in progress...)
  - The shortest and cutest. Continues on part/3 where you now reogranize the front facing Web API application and introduce new Controller code to expose a friendly REST-like API to server desired JSON to a client query sent over HTTP(S). You will learn how to throttle, authenticate, and authorize requests. Also you will give the client an overview of your API through a `Swagger` library documentation page based upon your Controllers and Actions.

## Contribution

I am open to comments, issues, forks/PRs and everything of good concern and idea. We can also discuss your ideas and topics in the comments section on the blog post articles.

## Author

Vedran Mandić.