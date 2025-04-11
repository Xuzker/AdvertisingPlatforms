using AdvertisingPlatforms.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingPlatforms.Controllers
{
    /// <summary>
    /// Контроллер для загрузки и поиска рекламных платформ по локациям.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformInformation _platformInformation;

        public PlatformController(IPlatformInformation platformInformation)
        {
            _platformInformation = platformInformation;
        }

        /// <summary>
        /// Загружает данные о платформах из массива строк.
        /// </summary>
        /// <param name="lines">Массив строк, каждая из которых содержит платформу и локации.</param>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadAds(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                var lines = new List<string>();
                while (await reader.ReadLineAsync() is { } line)
                {
                    lines.Add(line);
                }

                _platformInformation.LoadFromFilePlatfroms(lines.ToArray());
                return Ok("Data loaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Возвращает список платформ, подходящих для указанной локации.
        /// </summary>
        /// <param name="location">Локация для поиска платформ.</param>
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Location is required.");

            var platforms = _platformInformation.GetPlatforms(location);
            return Ok(platforms);
        }
    }
}
