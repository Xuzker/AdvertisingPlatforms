using AdvertisingPlatforms.Interfaces;
using AdvertisingPlatforms.Services;
using Microsoft.AspNetCore.Http.Features;

namespace AdvertisingPlatforms
{
    /// <summary>
    /// ������� ����� ����������, ���������� ����� ����� � ������������ ���-�������
    /// </summary>
    public class Program
    {
        /// <summary>
        /// ����� ����� � ����������
        /// </summary>
        /// <param name="args">��������� ��������� ������</param>
        public static void Main(string[] args)
        {
            // ������� ��������� (builder) ���-����������
            var builder = WebApplication.CreateBuilder(args);

            // ������������ �������� (DI-����������)

            // ������������ ������ ������ � ���������� ���������� ��� singleton
            // ��� ��������� ������ ��������� ������� �� ��� ����� ������ ����������
            builder.Services.AddSingleton<IPlatformInformation, PlatformService>();

            // ��������� ��������� ������������ (MVC)
            builder.Services.AddControllers();

            // ��������� ��������� ������������ API (Swagger)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ����������� ��������� ��� �������� ������
            builder.Services.Configure<FormOptions>(options =>
            {
                // ������������� ������������ ������ ������������ �����
                // long.MaxValue �������� ����������� �������������� ������
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            // ������ ����������
            var app = builder.Build();

            // ������������ ��������� ��������� HTTP-��������

            // �������� Swagger UI ������ � ������ ����������
            if (app.Environment.IsDevelopment())
            {
                // �������� middleware ��� ��������� JSON-������������ Swagger
                app.UseSwagger();

                // �������� middleware ��� Swagger UI (������������� ������������)
                app.UseSwaggerUI();
            }

            // ��������� middleware ��� ��������������� HTTP �� HTTPS
            app.UseHttpsRedirection();

            // ��������� middleware ��� �����������
            app.UseAuthorization();

            // ������� ��������� � ������������
            app.MapControllers();

            // ��������� ����������
            app.Run();
        }
    }
}