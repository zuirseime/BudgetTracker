using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BudgetTracker.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "First name is requiered")] public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Last name is requiered")] public string LastName { get; set; } = null!;
        [NotMapped] public string FullName => $"{FirstName} {LastName}";

        public DateOnly RegistationDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}