using AdvertisingPlatforms.Interfaces;
using AdvertisingPlatforms.Services;
using Microsoft.AspNetCore.Http.Features;

namespace AdvertisingPlatforms
{
    /// <summary>
    /// Главный класс приложения, содержащий точку входа и конфигурацию веб-сервиса
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Точка входа в приложение
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        public static void Main(string[] args)
        {
            // Создаем строитель (builder) веб-приложения
            var builder = WebApplication.CreateBuilder(args);

            // Конфигурация сервисов (DI-контейнера)

            // Регистрируем сервис работы с рекламными площадками как singleton
            // Это обеспечит единый экземпляр сервиса на все время работы приложения
            builder.Services.AddSingleton<IPlatformInformation, PlatformService>();

            // Добавляем поддержку контроллеров (MVC)
            builder.Services.AddControllers();

            // Добавляем генератор документации API (Swagger)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Настраиваем параметры для загрузки файлов
            builder.Services.Configure<FormOptions>(options =>
            {
                // Устанавливаем максимальный размер загружаемого файла
                // long.MaxValue означает практически неограниченный размер
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            // Строим приложение
            var app = builder.Build();

            // Конфигурация конвейера обработки HTTP-запросов

            // Включаем Swagger UI только в режиме разработки
            if (app.Environment.IsDevelopment())
            {
                // Включаем middleware для генерации JSON-спецификации Swagger
                app.UseSwagger();

                // Включаем middleware для Swagger UI (интерактивная документация)
                app.UseSwaggerUI();
            }

            // Добавляем middleware для перенаправления HTTP на HTTPS
            app.UseHttpsRedirection();

            // Добавляем middleware для авторизации
            app.UseAuthorization();

            // Маппинг маршрутов к контроллерам
            app.MapControllers();

            // Запускаем приложение
            app.Run();
        }
    }
}