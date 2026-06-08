using System.Net;

namespace FinalYearProject.Data.Utilities;

public class ServiceResponse<T>
{
    /// <summary>
    /// Message to be returned back to the API client.
    /// It could either be a failure message or a prompt to notify the api client about a successful operation being carried out
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Agreed Http Status Code to be sent in controller as a response
    /// </summary>
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    /// <summary>
    /// Data returned from the public service method.
    /// </summary>
    public T? Data { get; set; }
}

public class ServiceResponse
{

    /// <summary>
    /// Message to be returned back to the API client.
    /// It could either be a failure message or a prompt to notify the api client about a successful operation being carried out
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Agreed Http Status Code to be sent in controller as a response
    /// </summary>
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
}
