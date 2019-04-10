using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress, DisplayName("E-mail")]
        public string Email { get; set; }
    }
}
