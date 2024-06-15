using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Views.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "The email field is required.")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "The password field is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; } = false;
    }
}