using AdvertisingPlatforms.Interfaces;
using AdvertisingPlatforms.Models;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace AdvertisingPlatforms.Services
{
    /// <summary>
    /// Сервис загрузки и поиска рекламных платформ по локациям.
    /// </summary>
    public class PlatformService : IPlatformInformation
    {
        private readonly ConcurrentDictionary<string, List<Platform>> _platforms = new();
        private readonly object _lock = new();
        private readonly ILogger<PlatformService> _logger;

        public PlatformService(ILogger<PlatformService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Загружает данные о платформах из текстового массива.
        /// </summary>
        public void LoadFromFilePlatfroms(string[] lines)
        {
            var maps = new ConcurrentDictionary<string, List<Platform>>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains(':'))
                {
                    _logger.LogWarning("Пропущена строка: неверный формат — '{Line}'", line);
                    continue;
                }

                var info = line.Split(':', 2);
                var platformName = info[0].Trim();
                var locations = info[1].Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(location => location.Trim())
                    .Where(location => !string.IsNullOrWhiteSpace(location));

                var platform = new Platform
                {
                    Name = platformName,
                    Locations = locations.ToList()
                };

                foreach (var location in locations)
                {
                    if (!location.StartsWith('/'))
                    {
                        _logger.LogWarning("Пропущена локация без ведущего слеша: '{Location}'", location);
                        continue;
                    }

                    maps.AddOrUpdate(location, new List<Platform> { platform }, (key, old) =>
                    {
                        old.Add(platform);
                        return old;
                    });
                }
            }

            lock (_lock)
            {
                _platforms.Clear();
                foreach (var pair in maps)
                    _platforms[pair.Key] = pair.Value;
                _logger.LogInformation("Загрузка платформ завершена. Загружено {Count} локаций.", _platforms.Count);
            }
        }


        /// <summary>
        /// Возвращает список платформ, доступных для данной локации или любой из её родительских локаций.
        /// </summary>
        public List<Platform> GetPlatforms(string location)
        {
            var result = new HashSet<Platform>();
            foreach (var loc in BuildLocationHierarchy(location))
            {
                if (_platforms.TryGetValue(loc, out var platforms))
                {
                    foreach (var platform in platforms)
                        result.Add(platform);
                }
            }
            _logger.LogInformation("Найдено {Count} платформ для локации '{Location}'", result.Count, location);
            return result.OrderBy(p => p.Name).ToList();
        }

        /// <summary>
        /// Возвращает все иерархические уровни локации (например, /ru/svrd/revda → /ru/svrd/revda, /ru/svrd, /ru).
        /// </summary>
        private IEnumerable<string> BuildLocationHierarchy(string location)
        {
            var paths = location.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (int i = paths.Length; i >= 1; i--)
            {
                yield return "/" + string.Join('/', paths.Take(i));
            }
        }
    }
}
