using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MedsProcessor.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MedsProcessor.WebAPI.Infrastructure
{
  public static class ApiResponse
  {
    public static ObjectResult ForCode(int statusCode) =>
      new ObjectResult(
        new ApiHttpResponse(statusCode))
      {
        StatusCode = statusCode
      };

    public static ObjectResult ForMessage(string message, int statusCode = 200) =>
      new ObjectResult(
        new ApiMessageResponse(statusCode, message))
      {
        StatusCode = statusCode
      };

    public static ObjectResult ForData<TObjectModel>(
        TObjectModel model,
        int statusCode = 200,
        string message = null) =>
      new ObjectResult(
        new ApiDataResponse<TObjectModel>(statusCode, model, message))
      {
        StatusCode = statusCode
      };

    public static ObjectResult ForPagedData<TObjectModel>(
        IEnumerable<TObjectModel> model,
        int page,
        int size,
        int statusCode = 200,
        string message = null) =>
      new ObjectResult(
        new ApiPagedDataResponse<IEnumerable<TObjectModel>>(
          statusCode,
          GetPaged(page, size, model),
          page,
          size,
          message))
      {
        StatusCode = statusCode
      };

    public static ObjectResult TryForPagedData<TObjectModel>(
        IEnumerable<TObjectModel> model,
        int? page = null,
        int? size = null,
        int statusCode = 200,
        string messageForPagedData = null) =>
      page.HasValue && size.HasValue ?
      ForPagedData(
        model,
        page.Value,
        size.Value,
        statusCode,
        messageForPagedData) :
      ForData(
        model,
        statusCode,
        message: "To get paged data sepcify both 'page' and 'size' query params.");

    private static IEnumerable<TObjectModel> GetPaged<TObjectModel>(
        int page,
        int size,
        IEnumerable<TObjectModel> model) =>
      model
      .Skip((page - 1) * size)
      .Take(size);
  }

  public class ApiDataResponse<TObjectModel> : ApiMessageResponse
  {
    public TObjectModel Data { get; }

    public ApiDataResponse(int statusCode, TObjectModel model, string message = null) : base(statusCode, message)
    {
      if (model == default)
      {
        throw new ArgumentNullException("Response object model data can not be null or of default value for the given model data.");
      }

      Data = model;
    }
  }

  /// <summary>
  /// For the sake of the tutorial, this class will wrap over HTTP header data and provide the HTTP status code and its description.
  /// </summary>
  /// <remarks>Do not consider using this class at all in production environments. Your base class can instead be <see cref="ApiMessageResponse" /> which can include an additional <c>IsError</c> boolean property indicating if the HTTP request processing errored.</remarks>
  public class ApiHttpResponse
  {
    public int StatusCode { get; }

    public string StatusDescription { get; }

    public ApiHttpResponse(int statusCode)
    {
      if (!Enum.IsDefined(typeof(HttpStatusCode), statusCode))
      {
        throw new ArgumentOutOfRangeException(nameof(statusCode), statusCode, "The provided HTTP status code is invalid.");
      }

      this.StatusCode = statusCode;
      this.StatusDescription = ((HttpStatusCode) statusCode).ToString();
    }
  }

  public class ApiMessageResponse : ApiHttpResponse
  {
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Message { get; }

    public ApiMessageResponse(int statusCode, string message) : base(statusCode)
    {
      this.Message = message;
    }
  }

  public class ApiPagedDataResponse<TObjectModel> : ApiDataResponse<TObjectModel>
  {
    public ApiPagedDataResponse(
      int statusCode,
      TObjectModel model,
      int pageNumber,
      int pageSize,
      string message = null) : base(statusCode, model, message)
    {
      if (pageNumber < 1)
      {
        throw new ArgumentOutOfRangeException(nameof(pageNumber));
      }

      if (pageSize < 1)
      {
        throw new ArgumentOutOfRangeException(nameof(pageSize));
      }

      PageNumber = pageNumber;
      PageSize = pageSize;
    }

    public int PageNumber { get; }
    public int PageSize { get; }
  }
}