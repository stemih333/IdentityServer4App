using CleanTasks.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{

    public class AdministrationInputModel
    {
        [HiddenInput]
        public int Page { get; set; } = 1;
        [HiddenInput]
        public int PageSize { get; set; } = 15;
        [HiddenInput]
        public string SortColumn { get; set; } = "UserName";
        [HiddenInput]
        public string SortOrder { get; set; } = "asc";
        [HiddenInput]
        public string Permission { get; set; } = AuthConstants.UserPermission;
        [StringLength(25), DisplayName("First name:")]
        public string FirstName { get; set; }
        [StringLength(25), DisplayName("Last name:")]
        public string LastName { get; set; }
        [StringLength(25), DisplayName("User name:")]
        public string UserName { get; set; }
        [StringLength(100), DisplayName("E-mail address:")]
        public string Email { get; set; }
        [StringLength(25), DisplayName("Created by:")]
        public string CreatedBy { get; set; }
        [DisplayName("Created date:")]
        public DateTime? Created { get; set; }
        [StringLength(25), DisplayName("Updated by:")]
        public string UpdatedBy { get; set; }
        [DisplayName("Updated date:")]
        public DateTime? Updated { get; set; }
    }
}
