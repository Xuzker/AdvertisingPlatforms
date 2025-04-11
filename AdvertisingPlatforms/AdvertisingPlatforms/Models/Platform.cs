namespace AdvertisingPlatforms.Models
{
    public class Platform
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Locations { get; set; } = new();
    }
}
