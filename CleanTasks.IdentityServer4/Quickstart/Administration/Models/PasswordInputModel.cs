using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class PasswordInputModel
    {
        [Required]
        public string UserId { get; set; }

        [Required, StringLength(30, MinimumLength = 8)]
        public string NewPassword { get; set; }

        [Required, StringLength(30, MinimumLength = 8)]
        public string OldPassword { get; set; }

        [Required, StringLength(30, MinimumLength = 8)]
        public string ConfirmNewPassword { get; set; }
    }
}
