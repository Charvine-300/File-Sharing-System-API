
using FinalYearProject.Data.Utilities;
using System.Net;

namespace FinalYearProject.Services.Shared;


/// <summary>
/// A class to return an generic instance of ServiceResponse<T> or just instance of Service Response
/// </summary>
public class Response
{
    /// <summary>
    /// Return a generic instance of <see cref="ServiceResponse{T}"/> with message, data and StatusCode of 404
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ServiceResponse<T> NotFound<T>(string message, T data)
    {
        ServiceResponse<T> response = new()
        {
            StatusCode = HttpStatusCode.NotFound,
            Message = message,
            Data = data,
        };
        return response;
    }


    /// <summary>
    /// Returns an instance of <see cref="ServiceResponse"/> with a message from the response parameter and StatusCode of 404
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ServiceResponse NotFound(string message)
    {
        ServiceResponse response = new()
        {
            StatusCode = HttpStatusCode.NotFound,
            Message = message,
        };
        return response;
    }


    /// <summary>
    /// Return a generic instance of <see cref="ServiceResponse{T}"/> with message, data and StatusCode of 409
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ServiceResponse<T> Conflict<T>(string message, T data)
    {
        ServiceResponse<T> response = new()
        {
            StatusCode = HttpStatusCode.Conflict,
            Message = message,
            Data = data,
        };
        return response;
    }


    /// <summary>
    /// Returns an instance of <see cref="ServiceResponse"/> with a message from the response parameter and StatusCode of 409
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ServiceResponse Conflict(string message)
    {
        ServiceResponse response = new()
        {
            StatusCode = HttpStatusCode.Conflict,
            Message = message,
        };
        return response;
    }

    /// <summary>
    /// Returns a generic response of <see cref="ServiceResponse{T}"/> with message, data, and StatusCode of 400
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ServiceResponse<T> BadRequest<T>(string message, T data)
    {
        ServiceResponse<T> response = new()
        {
            StatusCode = HttpStatusCode.BadRequest,
            Message = message,
            Data = data
        };
        return response;
    }

    /// <summary>
    /// Returns a bad request response of <see cref="ServiceResponse"/> instance with message, and StatusCode of 400
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ServiceResponse BadRequest(string message)
    {
        ServiceResponse response = new()
        {
            StatusCode = HttpStatusCode.BadRequest,
            Message = message
        };
        return response;
    }

    /// <summary>
    /// Returns a Ok response  of <see cref="ServiceResponse{T}"/> , data, and StatusCode of 200
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ServiceResponse<T> Success<T>(string message, T data) where T : class
    {
        ServiceResponse<T> response = new()
        {
            StatusCode = HttpStatusCode.OK,
            Message = message,
            Data = data
        };
        return response;
    }

    /// <summary>
    /// Returns a Ok response  of <see cref="ServiceResponse"/> and StatusCode of 200
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ServiceResponse Success(string message)
    {
        ServiceResponse response = new()
        {
            StatusCode = HttpStatusCode.OK,
            Message = message,
        };
        return response;
    }

    /// <summary>
    /// Returns a Created response  of <see cref="ServiceResponse{T}"/> , data, and StatusCode of 201
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ServiceResponse<T> Created<T>(string message, T data) where T : class
    {
        ServiceResponse<T> response = new()
        {
            StatusCode = HttpStatusCode.Created,
            Message = message,
            Data = data
        };
        return response;
    }

    /// <summary>
    /// Returns a Created response  of <see cref="ServiceResponse"/> and StatusCode of 201
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ServiceResponse Created(string message)
    {
        ServiceResponse response = new()
        {
            StatusCode = HttpStatusCode.Created,
            Message = message,
        };
        return response;
    }

    /// <summary>
    /// Returns a generic response of <see cref="ServiceResponse{T}"/> with message, data, and StatusCode of 401
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ServiceResponse<T> Unauthorized<T>(string message, T data)
    {
        ServiceResponse<T> response = new()
        {
            StatusCode = HttpStatusCode.Unauthorized,
            Message = message,
            Data = data
        };
        return response;
    }

    /// <summary>
    /// Returns a bad request response of <see cref="ServiceResponse"/> instance with message, and StatusCode of 401
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ServiceResponse Unauthorized(string message)
    {
        ServiceResponse response = new()
        {
            StatusCode = HttpStatusCode.Unauthorized,
            Message = message
        };
        return response;
    }

    /// <summary>
    /// Returns a generic instance of <see cref="ServiceResponse{T}"/> with message,data and StatusCode of 403
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ServiceResponse<T> Forbidden<T>(string message, T data)
    {
        ServiceResponse<T> response = new()
        {
            StatusCode = HttpStatusCode.Forbidden,
            Message = message,
            Data = data
        };

        return response;
    }

    /// <summary>
    /// Returns an instace of <see cref="ServiceResponse"/> with message and StatusCode of 403
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ServiceResponse Forbidden(string message)
    {
        ServiceResponse response = new()
        {
            StatusCode = HttpStatusCode.Forbidden,
            Message = message,
        };
        return response;
    }


    /// <summary>
    /// Returns generic instance of <see cref="ServiceResponse{T}"/> with message and StatusCode of 500
    /// </summary>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ServiceResponse<T> SystemMalfunction<T>(string message, T data)
    {
        ServiceResponse<T> response = new()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = message,
            Data = data
        };
        return response;
    }

    /// <summary>
    /// Returns instace of <see cref="ServiceResponse"/> with message and StatusCode of 500
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ServiceResponse SystemMalfunction(string message)
    {
        ServiceResponse response = new()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = message
        };
        return response;
    }

}
