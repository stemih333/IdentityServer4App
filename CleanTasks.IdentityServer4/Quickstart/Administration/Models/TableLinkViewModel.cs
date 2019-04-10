namespace IdentityServer4.Quickstart.UI
{
    public class TableLinkViewModel
    {
        public int Page { get; set; }
        public string Action { get; set; }
        public AdministrationViewModel AdminModel { get; set; }
        public string Text { get; set; }
        public string SortColumn { get; set; }
    }
}
