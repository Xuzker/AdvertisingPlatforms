using AdvertisingPlatforms.Models;

namespace AdvertisingPlatforms.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для работы с рекламными площадками
    /// </summary>
    public interface IPlatformInformation
    {
        /// <summary>
        /// Загружает данные о площадках из массива строк
        /// </summary>
        /// <param name="lines">Массив строк в формате "Название:локация1,локация2"</param>
        /// <remarks>
        /// Каждая строка должна содержать название площадки и список локаций,
        /// разделенных двоеточием. Локации в строке разделяются запятыми.
        /// Пример: "Яндекс.Директ:/ru,/ru/msk"
        /// </remarks>
        void LoadFromFilePlatfroms(string[] lines);

        /// <summary>
        /// Возвращает список площадок, доступных для указанной локации
        /// </summary>
        /// <param name="location">Локация для поиска (например "/ru/msk")</param>
        /// <returns>
        /// Список площадок, действующих в указанной локации или любой из её родительских локаций.
        /// Сортируется по названию площадки.
        /// </returns>
        /// <remarks>
        /// Поиск учитывает иерархию локаций. Например, для локации "/ru/svrd/revda"
        /// будут возвращены площадки с локациями "/ru/svrd/revda", "/ru/svrd" и "/ru"
        /// </remarks>
        List<Platform> GetPlatforms(string location);
    }
}
