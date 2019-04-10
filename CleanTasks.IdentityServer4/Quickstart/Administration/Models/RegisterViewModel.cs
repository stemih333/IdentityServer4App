using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class RegisterViewModel
    {
        [Required, StringLength(10, MinimumLength = 6), DisplayName("User Name")]
        public string UserName { get; set; }
        [Required, EmailAddress, DisplayName("E-mail"), StringLength(50)]
        public string Email { get; set; }
        [Required, StringLength(25, MinimumLength = 2), DisplayName("First name")]
        public string FirstName { get; set; }
        [Required, StringLength(25, MinimumLength = 2), DisplayName("Last name")]
        public string LastName { get; set; }
        [Required, StringLength(20, MinimumLength = 8)]
        public string Password { get; set; }
    }
}
