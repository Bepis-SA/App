namespace Bepixplore.Cities
{
    public class CitySearchRequestDto
    {
        public string PartialName { get; set; }
        public string? Country { get; set; }
        public int? MinPopulation { get; set; }
        public bool IsPopularFilter { get; set; } = false;
    }
}