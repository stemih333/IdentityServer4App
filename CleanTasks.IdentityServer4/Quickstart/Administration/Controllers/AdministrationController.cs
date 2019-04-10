using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Authorization;
using IdentityModel;
using CleanTasks.Common.Constants;
using System.Net.Http;
using CleanTasks.IdentityServer4.Identity;

namespace IdentityServer4.Quickstart.UI
{
    [SecurityHeaders]
    [Authorize(Policy = Policies.All)]
    public class AdministrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public AdministrationController(UserManager<ApplicationUser> userManager, IHttpClientFactory clientFactory)
        {
            _userManager = userManager;
            _httpClientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult EmailSuccess()
        {
            return View();
        }

        [HttpGet, Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Index(AdministrationInputModel inputModel)
        {
            var model = await GetViewModel(inputModel);
            return View(model);
        }

        [HttpGet]
        public IActionResult Password([Required]string userId)
        {
            if (!ModelState.IsValid) return View("ViewModelError");

            return View("Password", userId);
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([Required]string token, [Required][EmailAddress]string email)
        {
            if (!ModelState.IsValid) return View("ViewModelError");

            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                ModelState.AddModelError("", $"Email address '{email}' is unknown.");
                return View("ViewModelError");
            }

            if(await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", $"Email address '{email}' is already confirmed.");
                return View("ViewModelError");
            }

            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if(!confirmResult.Succeeded)
            {
                SetModelStateErrors(confirmResult.Errors);
                return View("ViewModelError");
            }

            return View("EmailSuccess");
        }

        [HttpGet, Authorize(Policy = "AdminPolicy")]
        public IActionResult Register()
        {
            return View();
        }


        [HttpGet, Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Edit([Required]string userId)
        {
            if (!ModelState.IsValid) return View("ViewModelError");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "Could not find user with id: " + userId);
                return View("ViewModelError");
            }

            var model = new EditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Updated = user.Updated,
                UpdatedBy = user.UpdatedBy,
            };
            return View(model);
        }


        [HttpPost, Authorize(Policy = "AdminPolicy"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ModelState.AddModelError("", "Could not find user with id: " + model.Id);
                return View("ViewModelError");
            }

            user.Updated = DateTime.Now;
            user.UpdatedBy = User.Identity.Name ?? "Unknown";
            user.Email = model.Email;
            user.LastName = model.LastName;
            user.FirstName = model.FirstName;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                SetModelStateErrors(result.Errors);
                return View("ViewModelError");
            }

            TempData["SuccessMessage"] = "Successfully updated user";
            return RedirectToAction("Details", new { UserId = model.Id });
        }

        [HttpGet, Authorize(Policy = "AdminPolicy")]
        public IActionResult Delete([Required]string userId, [Required]string username)
        {
            if (!ModelState.IsValid) return View("ViewModelError");

            var model = new DeleteUserViewModel
            {
                UserId = userId,
                UserName = username
            };

            return View("Delete", model);
        }

        [HttpGet]
        public async Task<IActionResult> Details([Required]string userId)
        {
            if (!ModelState.IsValid) {
                return View("ViewModelError");
            };

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) user = await _userManager.FindByNameAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "Could not get user details for user with id: " + userId);
                return View("ViewModelError");
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var model = new UserDetailsViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Created = user.Created,
                CreatedBy = user.CreatedBy,
                Updated = user.Updated,
                UpdatedBy = user.UpdatedBy,
                Permissions = claims?.Where(_ => _.Type.Equals(AuthConstants.PermissionType)).Select(_ => _.Value)
            };

            return View("Details", model);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CreatedBy = User.Identity.Name ?? "Unknown",
                    UpdatedBy = User.Identity.Name ?? "Unknown"
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var claimResult =
                        await _userManager.AddClaimsAsync(user, new List<Claim>
                        {
                            new Claim(AuthConstants.PermissionType, AuthConstants.UserPermission),
                            new Claim(JwtClaimTypes.Name, model.UserName),
                            new Claim(JwtClaimTypes.GivenName, model.FirstName),
                            new Claim(JwtClaimTypes.FamilyName, model.LastName),
                            new Claim(JwtClaimTypes.Email, model.Email)
                        });

                    if (claimResult.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        var restoreUrl = Url.Action("ConfirmEmail",
                            ControllerContext.ActionDescriptor.ControllerName,
                            new { token, email = user.Email },
                            Request.Scheme);

                        System.IO.File.WriteAllText("confirmMail.txt", restoreUrl);

                        TempData["SuccessMessage"] = "Successfully added user. Email verification mail sent.";
                        return RedirectToAction("Index");
                    }
                    else
                        SetModelStateErrors(claimResult.Errors);
                }
                else
                    SetModelStateErrors(result.Errors);
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Password(PasswordInputModel model)
        {
            if (!ModelState.IsValid) return View("password", model.UserId);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "Could not find user with id: " + model.UserId);
                return View("ViewModelError");
            }

            if(!model.NewPassword.Equals(model.ConfirmNewPassword))
            {
                ModelState.AddModelError("", "New password and confirmation are not the same.");
                return View("Password", model.UserId);
            }

            var changePassword = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if(!changePassword.Succeeded)
            {
                SetModelStateErrors(changePassword.Errors);

                return View("Password", model.UserId);
            }

            TempData["SuccessMessage"] = "Successfully updated password";
            return RedirectToAction("Details", new { model.UserId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser([Required]string userId, [Required]string username)
        {
            if (!ModelState.IsValid) return View("ViewModelError");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "Could not find user with id: " + userId);
                return View("ViewModelError");
            }

            var result = await _userManager.DeleteAsync(user);

            if(!result.Succeeded)
            {
                SetModelStateErrors(result.Errors);
                return View("ViewModelError");
            }

            TempData["SuccessMessage"] = "Successfully deleted user with username: " + username + ".";
            return RedirectToAction("Index");
        }

        [HttpPost, Authorize(Policy = "AdminPolicy"), ValidateAntiForgeryToken]
        public IActionResult Filter(AdministrationInputModel inputModel)
        {
            inputModel.Page = 1;
            return RedirectToAction("Index", inputModel);
        }

        private async Task<AdministrationViewModel> GetViewModel(AdministrationInputModel inputModel)
        {
            var users = FilterUsers(
                await _userManager.GetUsersForClaimAsync(new Claim(AuthConstants.PermissionType, inputModel.Permission)), 
                inputModel);

            var permissionUsers = users.Select(_ => new ApplicationUserModel
            {
                Id = _.Id,
                Email = _.Email,
                FirstName = _.FirstName,
                LastName = _.LastName,
                UserName = _.UserName,
                Created = _.Created,
                CreatedBy = _.CreatedBy,
                Updated = _.Updated,
                UpdatedBy = _.UpdatedBy
            });

            if (inputModel != null)
            {
                if (!string.IsNullOrEmpty(inputModel.SortColumn))
                {
                    inputModel.SortOrder = inputModel.SortOrder.Equals("asc") ? "desc" : "asc";
                }

                permissionUsers = permissionUsers
                    .AsQueryable()
                    .OrderBy($"{inputModel.SortColumn} {inputModel.SortOrder}")
                    .Skip(inputModel.PageSize * (inputModel.Page - 1))
                    .Take(inputModel.PageSize);
            }

            return new AdministrationViewModel
            {
                Users = permissionUsers,
                TotalUsers = users.Count,
                Page = inputModel.Page,
                IsFirstPage = inputModel.Page == 1,
                LastPage = ((users.Count - 1) / inputModel.PageSize) + 1,
                IsLastPage = ((users.Count - 1) / inputModel.PageSize) + 1 == inputModel.Page,
                PageSize = inputModel.PageSize,
                Permission = inputModel.Permission,
                SortColumn = inputModel.SortColumn,
                SortOrder = inputModel.SortOrder,
                Created = inputModel.Created,
                CreatedBy = inputModel.CreatedBy,
                Email = inputModel.Email,
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                Updated = inputModel.Updated,
                UpdatedBy = inputModel.UpdatedBy,
                UserName = inputModel.UserName
            };
        }

        private void SetModelStateErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
        }

        private IList<ApplicationUser> FilterUsers(IEnumerable<ApplicationUser> users, AdministrationInputModel model)
        {
            if (users == null) return null;

            if(!string.IsNullOrEmpty(model.FirstName)) users = users.Where(_ => _.FirstName.StartsWith(model.FirstName));
            if(!string.IsNullOrEmpty(model.LastName)) users = users.Where(_ => _.LastName.StartsWith(model.LastName));
            if(!string.IsNullOrEmpty(model.Email)) users = users.Where(_ => _.Email.StartsWith(model.Email));
            if(!string.IsNullOrEmpty(model.UserName)) users = users.Where(_ => _.UserName.StartsWith(model.UserName));
            if(!string.IsNullOrEmpty(model.CreatedBy)) users = users.Where(_ => _.CreatedBy.StartsWith(model.CreatedBy));
            if(!string.IsNullOrEmpty(model.UpdatedBy)) users = users.Where(_ => _.UpdatedBy.StartsWith(model.UpdatedBy));
            if(model.Updated.HasValue) users = users.Where(_ => _.Updated.HasValue && _.Updated.Value.Date == model.Updated.Value);
            if(model.Created.HasValue) users = users.Where(_ => _.Created.HasValue && _.Created.Value.Date == model.Created.Value);
            
            return users.ToList();
        }       
    }
}