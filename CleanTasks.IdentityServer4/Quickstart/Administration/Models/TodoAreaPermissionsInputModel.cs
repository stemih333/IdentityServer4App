using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class TodoAreaPermissionsInputModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string PermissionToAdd { get; set; }
    }
}
