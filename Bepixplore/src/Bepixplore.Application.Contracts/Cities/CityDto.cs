namespace Bepixplore.Application.Contracts.Cities
{
    public class CityDto
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public uint Population { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}