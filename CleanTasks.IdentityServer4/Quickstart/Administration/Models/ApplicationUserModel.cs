using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ApplicationUserModel
    {
        [Required, HiddenInput]
        public string Id { get; set; }
        [Display(Name = "First name"), Required, StringLength(25)]
        public string FirstName { get; set; }
        [Display(Name = "Last name"), Required, StringLength(25)]
        public string LastName { get; set; }
        [Display(Name = "Username"), Required, StringLength(25)]
        public string UserName { get; set; }
        [Display(Name = "E-mail address"), Required, StringLength(100), EmailAddress]
        public string Email { get; set; }
        [HiddenInput]
        public string CreatedBy { get; set; }
        [HiddenInput]
        public DateTime? Created { get; set; }
        [HiddenInput]
        public string UpdatedBy { get; set; }
        [HiddenInput]
        public DateTime? Updated { get; set; }
    }
}
