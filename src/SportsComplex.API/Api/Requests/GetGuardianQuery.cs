namespace SportsComplex.API.Api.Requests
{
    public class GetGuardianQuery
    {
        public List<int> Ids { get; set; } = new();
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Count { get; set; }
        public string? OrderBy { get; set; }
        public bool Descending { get; set; } = false;
    }
}
