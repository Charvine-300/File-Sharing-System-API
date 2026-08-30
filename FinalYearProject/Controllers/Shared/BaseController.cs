
using FinalYearProject.Data.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FinalYearProject.Controllers.Shared
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult ParseResult(ServiceResult response)
        {
            if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                return base.Ok(response);
            }
            else if (response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                return base.BadRequest(response);
            }
            else
            {
                throw new InvalidOperationException("Unsupported Result Status");
            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult ComputeApiResponse<T>(ServiceResponse<T> serviceResponse) where T : class
        {

            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    ApiRecordResponse<T> response = new(true, (int)HttpStatusCode.OK, serviceResponse.Message, serviceResponse.Data);
                    return Ok(response);

                case HttpStatusCode.Created:
                    response = new(true, (int)HttpStatusCode.OK, serviceResponse.Message, serviceResponse.Data);
                    return Created("", response);

                case HttpStatusCode.Unauthorized:
                    response = new(false, (int)HttpStatusCode.Unauthorized, serviceResponse.Message, serviceResponse.Data);
                    return Unauthorized(response);

                case HttpStatusCode.NotFound:
                    response = new(false, (int)HttpStatusCode.NotFound, serviceResponse.Message, serviceResponse.Data);
                    return NotFound(response);

                case HttpStatusCode.Conflict:
                    response = new(false, (int)HttpStatusCode.Conflict, serviceResponse.Message, serviceResponse.Data);
                    return Conflict(response);

                case HttpStatusCode.BadRequest:
                    response = new(false, (int)HttpStatusCode.BadRequest, serviceResponse.Message, serviceResponse.Data);
                    return BadRequest(response);
                case HttpStatusCode.Forbidden:
                    response = new(false, (int)HttpStatusCode.Forbidden, serviceResponse.Message, serviceResponse.Data);
                    return StatusCode((int)HttpStatusCode.Forbidden, response);
                case HttpStatusCode.InternalServerError:
                    response = new(false, (int)HttpStatusCode.BadRequest, serviceResponse.Message, serviceResponse.Data);
                    return BadRequest(response);
                default:
                    throw new ArgumentOutOfRangeException("HTTP Status Code Could Not Be Deciphered", nameof(serviceResponse.StatusCode));

            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult ComputeResponse<T>(ServiceResponse<T> serviceResponse) where T : class
        {
            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    ApiResponse<T> response = new(true, (int)HttpStatusCode.OK, serviceResponse.Message, serviceResponse.Data);
                    return Ok(response);

                case HttpStatusCode.Created:
                    response = new(true, (int)HttpStatusCode.Created, serviceResponse.Message, serviceResponse.Data);
                    return Created("", response);

                case HttpStatusCode.Unauthorized:
                    response = new(false, (int)HttpStatusCode.Unauthorized, serviceResponse.Message, serviceResponse.Data);
                    return Unauthorized(response);

                case HttpStatusCode.NotFound:
                    response = new(false, (int)HttpStatusCode.NotFound, serviceResponse.Message, serviceResponse.Data);
                    return NotFound(response);

                case HttpStatusCode.Conflict:
                    response = new(false, (int)HttpStatusCode.Conflict, serviceResponse.Message, serviceResponse.Data);
                    return Conflict(response);

                case HttpStatusCode.BadRequest:
                    response = new(false, (int)HttpStatusCode.BadRequest, serviceResponse.Message, serviceResponse.Data);
                    return BadRequest(response);
                case HttpStatusCode.Forbidden:
                    response = new(false, (int)HttpStatusCode.Forbidden, serviceResponse.Message, serviceResponse.Data);
                    return StatusCode((int)HttpStatusCode.Forbidden, response);
                case HttpStatusCode.InternalServerError:
                    response = new(false, (int)HttpStatusCode.BadRequest, serviceResponse.Message, serviceResponse.Data);
                    return BadRequest(response);
                default:
                    throw new ArgumentOutOfRangeException("HTTP Status Code Could Not Be Deciphered", nameof(serviceResponse.StatusCode));

            }
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult ComputeResponse(ServiceResponse serviceResponse)
        {
            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    ApiResponse response = new(true, (int)HttpStatusCode.OK, serviceResponse.Message);
                    return Ok(response);

                case HttpStatusCode.Created:
                    response = new(true, (int)HttpStatusCode.Created, serviceResponse.Message);
                    return Created("", response);

                case HttpStatusCode.Unauthorized:
                    response = new(false, (int)HttpStatusCode.Unauthorized, serviceResponse.Message);
                    return Unauthorized(response);

                case HttpStatusCode.NotFound:
                    response = new(false, (int)HttpStatusCode.NotFound, serviceResponse.Message);
                    return NotFound(response);

                case HttpStatusCode.Conflict:
                    response = new(false, (int)HttpStatusCode.Conflict, serviceResponse.Message);
                    return Conflict(response);

                case HttpStatusCode.BadRequest:
                    response = new(false, (int)HttpStatusCode.BadRequest, serviceResponse.Message);
                    return BadRequest(response);
                case HttpStatusCode.Forbidden:
                    response = new(false, (int)HttpStatusCode.Forbidden, serviceResponse.Message);
                    return StatusCode((int)HttpStatusCode.Forbidden, response);
                case HttpStatusCode.InternalServerError:
                    response = new(false, (int)HttpStatusCode.InternalServerError, serviceResponse.Message);
                    return BadRequest(response);
                default:
                    throw new ArgumentOutOfRangeException("HTTP Status Code Could Not Be Deciphered", nameof(serviceResponse.StatusCode));

            }
        }


        /// <summary>
        /// Convert string Id to guid. Use this to get Id of user or corporateId from the token.
        /// </summary>
        /// <param name="key">Key for id to fetch. e.g userId or corporateId</param>
        /// <returns></returns>
        /// 
        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Guid GetId(string key)
        {
            string id = HttpContext.User.FindFirst(key)?.Value ?? string.Empty;

            if (string.IsNullOrWhiteSpace(id))
            {
                HttpContext.Abort();
            }

            if (!Guid.TryParse(id, out Guid guid))
            {
                HttpContext.Abort();
            }
            return guid;
        }
    }
}
