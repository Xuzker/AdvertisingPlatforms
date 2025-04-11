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
        public IActionResult UploadAds([FromBody] string[] lines)
        {
            if (lines == null || lines.Length == 0)
                return BadRequest("No data provided.");

            _platformInformation.LoadFromFilePlatfroms(lines);
            return Ok("Data loaded successfully.");
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
