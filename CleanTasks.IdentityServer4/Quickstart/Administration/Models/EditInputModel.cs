using System.Collections.Generic;

namespace IdentityServer4.Quickstart.UI
{
    public class EditInputModel : ApplicationUserModel
    {
        public IEnumerable<string> TodoAreaPermissions { get; set; }
    }
}
