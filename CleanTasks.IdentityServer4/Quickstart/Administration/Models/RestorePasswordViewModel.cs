using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class RestorePasswordViewModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string Email { get; set; }
        [Required, DisplayName("New password")]
        public string NewPassword { get; set; }
        [Required, DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}
