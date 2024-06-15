using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Views.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "The first name field is required.")]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "The last name field is required.")]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "The email field is required.")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "The password field is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "The password confirmation field is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}