using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace CleanTasks.IdentityServer4.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(25)]
        public string FirstName { get; set; }
        [Required, StringLength(25)]
        public string LastName { get; set; }
        [Required]
        public DateTime? Created { get; set; }
        [Required]
        public DateTime? Updated { get; set; }
        [Required, StringLength(50)]
        public string UpdatedBy { get; set; }
        [Required, StringLength(50)]
        public string CreatedBy { get; set; }
    }
}
