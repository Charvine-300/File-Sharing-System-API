using FinalYearProject.Services.Decryption;
using FinalYearProject.Services.Encryption;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(IEncryptionService encryptionService) : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("encrypt")]
        public async Task<IActionResult> Encrypt(EncryptRequest request)
        {
            var result = encryptionService.Encrypt(request.Message);

            return Ok(new
            {
                cipherText = result.payload,
                aesKey = result.aesKey
            });
        }

        [HttpPost("decrypt")]
        public async Task<IActionResult> Decrypt(DecryptRequest request)
        {
            try
            {
                string result =
                     encryptionService.Decrypt(
                        request.CipherText
                    );

                return Ok(new
                {
                    Message = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Decryption failed",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("debug/native")]
        public IActionResult DebugNative()
        {
            return Ok(new
            {
                Is64Bit = Environment.Is64BitProcess,
                BaseDir = AppContext.BaseDirectory,
                NativeExists = System.IO.File.Exists(
                    Path.Combine(AppContext.BaseDirectory, "Native", "librabe_ffi.dll")
                )
            });
        }
    }
}
