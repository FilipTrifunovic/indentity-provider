using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Model
{
    public class RegisterModel
    {
        [Required]
        [StringLength(100, MinimumLength = 4)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
