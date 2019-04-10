using System.Collections.Generic;

namespace IdentityServer4.Quickstart.UI
{
    public class AdministrationViewModel : AdministrationInputModel
    {
        public IEnumerable<ApplicationUserModel> Users { get; set; }
        public int TotalUsers { get; set; }
        public int LastPage { get; set; }
        public bool IsLastPage { get; set; }
        public bool IsFirstPage { get; set; }
        public bool IsPageSelected { get; set; }
    }
}
