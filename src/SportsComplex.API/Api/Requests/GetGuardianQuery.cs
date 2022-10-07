namespace SportsComplex.API.Api.Requests
{
    public class GetGuardianQuery
    {
        public List<int> Ids { get; set; } = new();
        public int? Count { get; set; }
        public string? OrderBy { get; set; }
        public bool Descending { get; set; } = false;
    }
}
