using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FinalYearProject.Services.Validators
{
    /// <summary>
    /// Filters model validation error
    /// </summary>
    public static class ModelValidation
    {

        /// <summary>
        /// Injected in model validation
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public static IActionResult ValidateModels(ActionContext actionContext)
        {
            List<string> errors = [];

            var modelErrors = actionContext.ModelState.AsEnumerable();

            foreach (var error in modelErrors)
            {
                foreach (var message in error.Value!.Errors)
                    errors.Add(message.ErrorMessage);
            }
            return new BadRequestObjectResult(new
            {
                IsSuccessful = false,
                Message = errors.FirstOrDefault(),
                StatusCode = (int)HttpStatusCode.BadRequest,
                Data = errors,
            });
        }
    }
}
