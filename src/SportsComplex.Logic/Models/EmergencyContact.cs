using SportsComplex.Logic.Interfaces;

namespace SportsComplex.Logic.Models;

public class EmergencyContact : IModel
{
    public int Id { get; set; }
    public int GuardianId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string? OtherAddress { get; set; }
    public string? OtherPhoneNumber { get; set; }
}