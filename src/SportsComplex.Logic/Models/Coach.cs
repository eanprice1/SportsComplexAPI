namespace SportsComplex.Logic.Models;

public class Coach
{
    public int Id { get; set; }
    public int? TeamId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public bool IsHeadCoach { get; set; }
}