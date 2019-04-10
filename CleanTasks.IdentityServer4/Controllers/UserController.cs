using CleanTasks.Common.Constants;
using CleanTasks.IdentityServer4.Identity;
using CleanTasks.IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleanTasks.IdentityServer4.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize(AuthenticationSchemes = "Bearer,Identity.Application")]
    public class UserController
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
            
        [HttpGet()]
        public async Task<List<UserDto>> Get()
            => (await _userManager.GetUsersForClaimAsync(new Claim(AuthConstants.PermissionType, AuthConstants.UserPermission)))?
            .Select(MapFromApplicationUser)
            .ToList();

        [HttpGet("{id:int?}")]
        public async Task<List<UserDto>> Get([Required]int? id)
            => (await _userManager.GetUsersForClaimAsync(new Claim(PermissionTypes.TodoAreaPermission, id.Value.ToString())))?
            .Select(MapFromApplicationUser)
            .ToList();

        private UserDto MapFromApplicationUser(ApplicationUser user)
        => new UserDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Id = user.Id,
            UserName = user.UserName
        };
    }
}
