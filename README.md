# The build a `meds-processor` 💊 guide to C# and .NET Core

**This is the perfect place to start learning C# and .NET Core by building something real with looks and feels to it - a drug list data scraper and a secure documented REST web API.** This project is designed for developers who have moderate programming experience and some experience in building web apps but have not still encuntered C# and .NET on the backend.

## Updates

- 2019-12-02: updated to SDK 2.2.402, cleaned up, forced `ValidFrom.Year < 2019`, removed parallelization of parser due to file lock situations, cleaned up a bit the API responses, removed unnecessary code.
- 2019-10-19: The processor might be in a bit of a problem at the moment due to the fact that a new (fifth...) parser is required as a new .xls(x) scheme has been published for 2019. :-) ah, burocracy in Croatia, you gotta love it. A quick fix for you guys is to filter out documents by a `ValidFrom.Year < 2019` expression. I will be fixing this with the additional parser (or other fix) and updating the blog posts!

## Built with .NET Core SDK 🔧

The cross-platform production ready SDK is [.NET Core](https://dotnet.microsoft.com/download) and the version used to build this application is `"version": "2.1.403"`. You can find the SDK downloads for your OS [here](https://dotnet.microsoft.com/download/dotnet-core/2.1#sdk-2.1.403).

## Build, run & use 🏃

**Build** the application (and ensure internet connectivity for NuGet packages to restore) with:

```bash
> cd src/MedsProcessor.WebAPI
MedsProcessor.WebAPI> dotnet build
```

**Run** the application (and ensure internet connectivity for web scraper to work) on https://localhost:5001 with:

```bash
> cd src/MedsProcessor.WebAPI
MedsProcessor.WebAPI> dotnet run
```

You can now **browse the Web API via a Swagger UI** on the address: https://localhost:5001/swagger/index.html

## A drugs Web API (you'll build) 🤖

The image below is a screenshot of the Swagger UI which is produced to document the Web API with available endpoints and their respected HTTP methods.

![meds-processor-api swagger image](https://i.ibb.co/tXV63wf/meds-processor-api.png)

## Why (I built this) 🙈

I was irritated by the fact that my country's health insurance fund realeases important data such as medicine and drugs in such an unstructured and user unfriendly format. Also, I figured I am a bit rusty with .NET Core and writing technical blogs.

## About / Summary 🕵

Learn to build a web scraper, downloader & Excel parser by digging through some hiddious spreadsheet data of Croatia's Health Insurance Fund and its primary and supplementary list of drugs and all that by using only C# and .NET Core (on any modern computer OS platform)! The .NET Core SDK can be installed and used the same on Windows, OSX or Linux.

## Where to start (project guide) 🤔

The repository is composed of four parts. Those parts are **git branches** where each of them has their own blog post article. You can browse through branches here on GitHub (the branch selection dropdown). I advise that you start by reading the [blog part/1](https://medium.com/@vekzdran/practical-net-core-write-a-web-scraper-downloader-excel-parser-part-1-4-ece43e0af898) as it will guide you through building the solution on your own. You can use any modern OS and code editor.

### Branches / parts / articles 🌱

- **part/1** ([Practical .NET Core  🚀— write a web scraper, downloader & Excel parser. Part 1: Scraper](https://medium.com/@vekzdran/practical-net-core-write-a-web-scraper-downloader-excel-parser-part-1-4-ece43e0af898))
  - The thrilling one. Bootstraps you to the C# and .NET world. Sets you up with the whole project. Introduces you to .NET Core, the dotnet CLI where you will create a web scraper app using the `AngleSharp` library to fetch some remote HTML pages and extract some links.

- **part/2** ([Practical .NET Core 🚀— write a scraper, fetcher & xls(x) parser. Part 2: Downloader](https://medium.com/@vekzdran/practical-net-core-write-a-scraper-fetcher-xls-x-parser-part-2-parallel-downloading-fc4d21f21417))
  - The most exciting one. Continues on part/1 where now you will download the documents to which the former links lead to. The documents are .xls(x) spreadsheets that you will save to your disk drive. You will learn how to utilized the `Task Parallel Library` and process async Tasks in .NET Core.

- **part/3** ([Practical .NET Core 🚀— write a scraper, fetcher & .xls(x) parser. Part 3: Parser](https://medium.com/@vekzdran/practical-net-core-build-a-scraper-fetcher-xls-x-parser-part-3-cross-platform-parser-657822ea8471))
  - The longest and thughest. Continues on part/2 where you have downloaded spreadsheet docs that need parsing into C# models. You will learn to use the `NPOI` spreadsheets parsing library to extract relevant data for your C# model classes. Upon finishing you will have a single dataset of transformed and organized data.

- **part/4** ([Practical .NET Core 🚀— write a scraper, fetcher & .xls(x) parser. Part 4: Secure REST web API](https://medium.com/@vekzdran/practical-net-core-write-a-web-scraper-fetcher-excel-parser-part-4-secure-rest-web-api-b07d002c0bac))
  - The shortest & cutest, jk, the longest & toughest! Continues on part/3 where you now organize the front facing web API app and introduce new Controller code to expose a friendly RESTful API serving JSON content to client requests sent over HTTP(S). You will learn how to throttle, authenticate (with JWT) and authorize requests. Also, you will give the client an overview of your API through an Open API spec with `Swagger` docs based upon your well documented Controllers and Actions.

## Caveats 🤕

The source has changed a lot through the parts. There might be bugs as this project is not covered by tests (something which I might consider in future). **Part/4 besides the Web API implementation goes into refactoring and improving some parts which are on purpose not in their best form from previous parts.** What you will first notice is that this README.md document is not in this final form on the first three branches. Don't get discouraged, rather notify me if you see place for improvement. Everything should work as expected if you follow the blog series. Also, all the practices you see here such as a base class for a HTTP response to carry HTTP header data are not the best production to use thing. So yeah, always stay curious, ask your self "Why?", rethink your approach and then execute.

## Contribution 💞

I am open to improvements, comments, issues, forks/PRs and everything of good concern and idea.
We can also discuss your ideas and topics in the comments section on the blog post articles if you prefer that.

## Author 🤓

Vedran Mandić.

## License 👀

MIT License
