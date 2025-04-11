using AdvertisingPlatforms.Models;
using AdvertisingPlatforms.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AdvertisingPlatforms.Tests
{
    /// <summary>
    /// ����� ��� ������� ������ � ���������� ���������� (PlatformService)
    /// </summary>
    public class PlatformServiceTests
    {
        private readonly Mock<ILogger<PlatformService>> _loggerMock;
        private readonly PlatformService _service;

        /// <summary>
        /// ������������� ����� ������ ������
        /// </summary>
        public PlatformServiceTests()
        {
            // ������� mock �������
            _loggerMock = new Mock<ILogger<PlatformService>>();

            // �������������� ����������� ������ � mock-��������
            _service = new PlatformService(_loggerMock.Object);
        }

        /// <summary>
        /// ���� �������� ���������� ������
        /// ��������� ��� ������ ����������� � ��������� ����������� � �������
        /// </summary>
        [Fact]
        public void LoadFromFilePlatforms_ValidData_LoadsPlatformsCorrectly()
        {
            // Arrange (����������)
            // ������� �������� ������ - ������ ����� � ����������� �������
            var lines = new[]
            {
                "������.������:/ru",  // �������� � ����� ��������
                "���������� �������:/ru/svrd/revda,/ru/svrd/pervik"  // �������� � ����������� ���������
            };

            // Act (��������)
            // �������� ����������� ����� �������� ������
            _service.LoadFromFilePlatfroms(lines);

            // Assert (��������)

            // ��������� ����� �� �������� ������� /ru
            var resultRu = _service.GetPlatforms("/ru");
            // ������ ���� ������ ���� ��������
            Assert.Single(resultRu);
            // ��������� ��� ��� ������ ������.������
            Assert.Equal("������.������", resultRu[0].Name);

            // ��������� ����� �� ��������� ������� /ru/svrd/revda
            var resultRevda = _service.GetPlatforms("/ru/svrd/revda");
            // ������ ���� 2 �������� (������ � ���������� �������)
            Assert.Equal(2, resultRevda.Count);
            // ��������� ������� ����� �������� � �����������
            Assert.Contains(resultRevda, p => p.Name == "������.������");
            Assert.Contains(resultRevda, p => p.Name == "���������� �������");
        }

        /// <summary>
        /// ���� ��������� ������������ ����� ��� ��������
        /// ��������� ��� ������ ��������� ������������ ������ ������� � �������� ��
        /// </summary>
        [Fact]
        public void LoadFromFilePlatforms_InvalidLines_LogsWarnings()
        {
            // Arrange (����������)
            // ������� �������� ������ � ������������� ��������
            var lines = new[]
            {
                "Invalid line without colon",  // ��� ����������� :
                "Another invalid line",       // ���� ��� �����������
                "Valid:/ru"                  // ���������� ������
            };

            // Act (��������)
            // �������� ����������� �����
            _service.LoadFromFilePlatfroms(lines);

            // Assert (��������)
            // ��������� ��� ��� ���� ������������ ����� ���� �������� warning-����
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,  // ������� ���� - Warning
                    It.IsAny<EventId>(),  // ����� EventId
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("�������� ������")),  // ��������� �������� �����
                    It.IsAny<Exception>(),  // ����� ���������� (��� null)
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),  // ������� ��������������
                Times.Exactly(2));  // ������ ���� ������� ����� 2 ����
        }

        /// <summary>
        /// ���� ������ �������� �� �������� �������
        /// ��������� ��� ����� ��������� ������������ �������
        /// </summary>
        [Fact]
        public void GetPlatforms_LocationHierarchy_ReturnsAllMatchingPlatforms()
        {
            // Arrange (����������)
            // ������� �������� ������ � ������� �������� �����������
            var lines = new[]
            {
                "������.������:/ru",           // �������� �������
                "������ �������:/ru/svrd",      // ������� �������� ������
                "���������� �������:/ru/svrd/revda"  // �������� �������
            };
            // ��������� ������
            _service.LoadFromFilePlatfroms(lines);

            // Act (��������)
            // ���� �������� ��� �������� �������
            var result = _service.GetPlatforms("/ru/svrd/revda");

            // Assert (��������)
            // ������ ���� 3 �������� (��� ���������� �� ��������)
            Assert.Equal(3, result.Count);
            // ��������� ������� ���� ��������� ��������
            Assert.Contains(result, p => p.Name == "������.������");  // ��������
            Assert.Contains(result, p => p.Name == "������ �������"); // ������� �������
            Assert.Contains(result, p => p.Name == "���������� �������"); // ������ ����������
        }

        /// <summary>
        /// ���� ������ �� ����������� �������
        /// ��������� ��� ��� ����������� ������� ������������ ������ ������
        /// </summary>
        [Fact]
        public void GetPlatforms_UnknownLocation_ReturnsEmptyList()
        {
            // Arrange (����������)
            // ��������� �������� ������ � ����� ���������
            _service.LoadFromFilePlatfroms(new[] { "������.������:/ru" });

            // Act (��������)
            // �������� ����� �������� ��� ����������� �������
            var result = _service.GetPlatforms("/unknown/location");

            // Assert (��������)
            // ��������� ������ ���� ������ �������
            Assert.Empty(result);
        }
    }
}