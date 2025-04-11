using AdvertisingPlatforms.Models;

namespace AdvertisingPlatforms.Interfaces
{
    public interface IPlatformInformation
    {
        void LoadFromFilePlatfroms(string[] lines);
        List<Platform> GetPlatforms(string location);
    }
}
