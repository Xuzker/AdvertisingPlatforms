namespace AdvertisingPlatforms.Models
{
    /// <summary>
    /// Класс, представляющий рекламную площадку с ее локациями
    /// </summary>
    public class Platform
    {
        /// <summary>
        /// Название рекламной площадки
        /// </summary>
        /// <example>"Яндекс.Директ"</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Список локаций, в которых действует площадка
        /// </summary>
        /// <remarks>
        /// Локации представляют собой иерархические пути, разделенные слешами,
        /// например: "/ru", "/ru/msk", "/ru/svrd/revda"
        /// </remarks>
        /// <example>["/ru", "/ru/msk"]</example>
        public List<string> Locations { get; set; } = new();
    }
}
