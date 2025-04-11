using AdvertisingPlatforms.Models;
using AdvertisingPlatforms.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AdvertisingPlatforms.Tests
{
    /// <summary>
    /// Тесты для сервиса работы с рекламными площадками (PlatformService)
    /// </summary>
    public class PlatformServiceTests
    {
        private readonly Mock<ILogger<PlatformService>> _loggerMock;
        private readonly PlatformService _service;

        /// <summary>
        /// Инициализация перед каждым тестом
        /// </summary>
        public PlatformServiceTests()
        {
            // Создаем mock логгера
            _loggerMock = new Mock<ILogger<PlatformService>>();

            // Инициализируем тестируемый сервис с mock-логгером
            _service = new PlatformService(_loggerMock.Object);
        }

        /// <summary>
        /// Тест загрузки корректных данных
        /// Проверяет что данные загружаются и правильно сохраняются в сервисе
        /// </summary>
        [Fact]
        public void LoadFromFilePlatforms_ValidData_LoadsPlatformsCorrectly()
        {
            // Arrange (подготовка)
            // Создаем тестовые данные - массив строк с корректными данными
            var lines = new[]
            {
                "Яндекс.Директ:/ru",  // Площадка с одной локацией
                "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik"  // Площадка с несколькими локациями
            };

            // Act (действие)
            // Вызываем тестируемый метод загрузки данных
            _service.LoadFromFilePlatfroms(lines);

            // Assert (проверки)

            // Проверяем поиск по корневой локации /ru
            var resultRu = _service.GetPlatforms("/ru");
            // Должна быть только одна площадка
            Assert.Single(resultRu);
            // Проверяем что это именно Яндекс.Директ
            Assert.Equal("Яндекс.Директ", resultRu[0].Name);

            // Проверяем поиск по вложенной локации /ru/svrd/revda
            var resultRevda = _service.GetPlatforms("/ru/svrd/revda");
            // Должно быть 2 площадки (Яндекс и Ревдинский рабочий)
            Assert.Equal(2, resultRevda.Count);
            // Проверяем наличие обеих площадок в результатах
            Assert.Contains(resultRevda, p => p.Name == "Яндекс.Директ");
            Assert.Contains(resultRevda, p => p.Name == "Ревдинский рабочий");
        }

        /// <summary>
        /// Тест обработки некорректных строк при загрузке
        /// Проверяет что сервис корректно обрабатывает ошибки формата и логирует их
        /// </summary>
        [Fact]
        public void LoadFromFilePlatforms_InvalidLines_LogsWarnings()
        {
            // Arrange (подготовка)
            // Создаем тестовые данные с некорректными строками
            var lines = new[]
            {
                "Invalid line without colon",  // Нет разделителя :
                "Another invalid line",       // Тоже нет разделителя
                "Valid:/ru"                  // Корректная строка
            };

            // Act (действие)
            // Вызываем тестируемый метод
            _service.LoadFromFilePlatfroms(lines);

            // Assert (проверки)
            // Проверяем что для двух некорректных строк были записаны warning-логи
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,  // Уровень лога - Warning
                    It.IsAny<EventId>(),  // Любой EventId
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("неверный формат")),  // Сообщение содержит текст
                    It.IsAny<Exception>(),  // Любое исключение (или null)
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),  // Функция форматирования
                Times.Exactly(2));  // Должно быть вызвано ровно 2 раза
        }

        /// <summary>
        /// Тест поиска площадок по иерархии локаций
        /// Проверяет что поиск учитывает родительские локации
        /// </summary>
        [Fact]
        public void GetPlatforms_LocationHierarchy_ReturnsAllMatchingPlatforms()
        {
            // Arrange (подготовка)
            // Создаем тестовые данные с разными уровнями вложенности
            var lines = new[]
            {
                "Яндекс.Директ:/ru",           // Корневая локация
                "Крутая реклама:/ru/svrd",      // Локация среднего уровня
                "Ревдинский рабочий:/ru/svrd/revda"  // Дочерняя локация
            };
            // Загружаем данные
            _service.LoadFromFilePlatfroms(lines);

            // Act (действие)
            // Ищем площадки для дочерней локации
            var result = _service.GetPlatforms("/ru/svrd/revda");

            // Assert (проверки)
            // Должно быть 3 площадки (все подходящие по иерархии)
            Assert.Equal(3, result.Count);
            // Проверяем наличие всех ожидаемых площадок
            Assert.Contains(result, p => p.Name == "Яндекс.Директ");  // Корневая
            Assert.Contains(result, p => p.Name == "Крутая реклама"); // Средний уровень
            Assert.Contains(result, p => p.Name == "Ревдинский рабочий"); // Точное совпадение
        }

        /// <summary>
        /// Тест поиска по неизвестной локации
        /// Проверяет что для неизвестной локации возвращается пустой список
        /// </summary>
        [Fact]
        public void GetPlatforms_UnknownLocation_ReturnsEmptyList()
        {
            // Arrange (подготовка)
            // Загружаем тестовые данные с одной площадкой
            _service.LoadFromFilePlatfroms(new[] { "Яндекс.Директ:/ru" });

            // Act (действие)
            // Пытаемся найти площадки для неизвестной локации
            var result = _service.GetPlatforms("/unknown/location");

            // Assert (проверки)
            // Результат должен быть пустым списком
            Assert.Empty(result);
        }
    }
}