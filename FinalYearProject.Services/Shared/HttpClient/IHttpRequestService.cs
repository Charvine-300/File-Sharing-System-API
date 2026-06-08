namespace FinalYearProject.Services.Shared.HTTPClient;


public interface IHttpRequestService
{
    string GetUTCTimestamp();
    Task<T> SendAsync<T>(
         HttpMethod method,
         string url,
         HttpContent? payload = null,
         Dictionary<string, string>? headers = null,
         string? bearerToken = null,
         string accept = "application/json",
         int timeout = 0
     ) where T : class;

}
