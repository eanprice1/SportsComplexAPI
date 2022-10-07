namespace SportsComplex.API.Api.Requests
{
    public class PlayerRequest
    {
        public int? TeamId { get; set; }
        public int GuardianId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
