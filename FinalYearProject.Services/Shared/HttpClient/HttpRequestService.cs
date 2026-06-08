using FinalYearProject.Services.Shared.HTTPClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection;

namespace FinalYearProject.Services.Shared;

public class HttpRequestService(ILogger<HttpRequestService> logger, HttpClient httpClient) : IHttpRequestService
{


    private T SetHttpResponseMessageOnResponseObject<T>(T responseObject, HttpResponseMessage responseMessage)
    {
        if (responseObject == null)
        {
            responseObject = Activator.CreateInstance<T>();
        }
        try
        {
            var type = typeof(T);
            if (!type.GetInterfaces().Contains(typeof(IHttpResponseMessage)))
            {
                return responseObject;
            }

            var prop = responseObject!.GetType()
                .GetProperty(nameof(IHttpResponseMessage.ResponseMessage), BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(responseObject, responseMessage);
            }
        }
        catch (Exception ex)
        {
            logger.LogError("An exception occurred while trying to set HttpResponseMessage on response object \nExceptionMessage : {Message}", ex.Message);
        }

        return responseObject;
    }

    public async Task<T> SendAsync<T>(
     HttpMethod method,
     string url,
     HttpContent? payload = null,
     Dictionary<string, string>? headers = null,
     string? bearerToken = null,
     string accept = "application/json",
     int timeout = 10_000
 ) where T : class
    {
        timeout = CheckTimeout(timeout);

        using var request = new HttpRequestMessage(method, url);
        if(accept == "application/json")
        {
              request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
        }

        if (bearerToken != null)
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);

        if (headers != null)
            foreach (var h in headers)
                request.Headers.TryAddWithoutValidation(h.Key, h.Value);

        request.Content = payload;
        try
        {
            using var cts = new CancellationTokenSource(timeout);
            HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            string content = await response.Content.ReadAsStringAsync();
            logger.LogInformation("Response from {requestUri} was {@content}", url, content);

            if (string.IsNullOrWhiteSpace(content))
            {
                return SetHttpResponseMessageOnResponseObject(default(T), response)!;
            }

            T? responseObject = JsonConvert.DeserializeObject<T>(content);
            responseObject = SetHttpResponseMessageOnResponseObject(responseObject, response);
            return responseObject!;
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred ---- {Message}", ex.Message);
        }
        return default!;
    }

    private static int CheckTimeout(int timeout)
    {
        if (timeout <= 0)
        {
            timeout = 60000;
        }

        return timeout;
    }

    public string GetUTCTimestamp()
    {
        DateTime utcdate = DateTime.UtcNow;
        string date = utcdate.ToString("yyyy-MM-ddHHmmss");
        return date;
    }

}