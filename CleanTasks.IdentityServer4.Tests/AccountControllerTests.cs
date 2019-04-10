using CleanTasks.IdentityServer4.Identity;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CleanTasks.IdentityServer4.Tests
{
    public class AccountControllerTests
    {
        [Fact]
        public async Task ForgotPassword_Success()
        {
            var viewModel = new ForgotPasswordViewModel
            {
                Email = "email@email.com"
            };

            var user = new ApplicationUser
            {
                Email = "email@email.com"
            };

            var userManager = MockTestUserManager<ApplicationUser>();
            userManager.Setup(_ => _.FindByEmailAsync(viewModel.Email)).ReturnsAsync(user);
            userManager.Setup(_ => _.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("Token");

            var controller = GetController(userManager.Object);

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("callbackUrl")
                .Verifiable();

            controller.Url = mockUrlHelper.Object;

            var result = await controller.ForgotPassword(viewModel);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task ForgotPassword_Fail_UnknownUser()
        {
            var viewModel = new ForgotPasswordViewModel
            {
                Email = "email@email.com"
            };

            var userManager = MockTestUserManager<ApplicationUser>();
            userManager.Setup(_ => _.FindByEmailAsync(viewModel.Email)).ReturnsAsync(null as ApplicationUser);

            var controller = GetController(userManager.Object);

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("callbackUrl")
                .Verifiable();

            controller.Url = mockUrlHelper.Object;

            var result = await controller.ForgotPassword(viewModel);

            Assert.IsType<ViewResult>(result);
            Assert.True(controller.ModelState.Count > 0);
            Assert.True(controller.ModelState.Keys.FirstOrDefault(_ => _.Equals("Unknown E-mail address")) != null);
            mockUrlHelper.Verify(_ => _.Action(It.IsAny<UrlActionContext>()), Times.Never());
        }

        [Fact]
        public async Task ForgotPassword_Fail_InvalidMail()
        {
            var viewModel = new ForgotPasswordViewModel
            {
                Email = "email"
            };

            var userManager = MockTestUserManager<ApplicationUser>();
           
            var controller = GetController(userManager.Object);

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("callbackUrl")
                .Verifiable();

            controller.Url = mockUrlHelper.Object;

            var result = await controller.ForgotPassword(viewModel);

            Assert.IsType<ViewResult>(result);
            Assert.True(controller.ModelState.Count > 0);
            mockUrlHelper.Verify(_ => _.Action(It.IsAny<UrlActionContext>()), Times.Never());
        }


        public AccountController GetController(UserManager<ApplicationUser> userManager)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "test";

            var controller = new AccountController(userManager, null, null, null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    ActionDescriptor = new ControllerActionDescriptor(),
                    HttpContext = httpContext
                }
            };
            controller.ControllerContext.ActionDescriptor.ControllerName = "AccountController";
            
            return controller;
        }

        public static Mock<UserManager<TUser>> MockTestUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;
            options.Setup(o => o.Value).Returns(idOptions);
            var userValidators = new List<IUserValidator<TUser>>();
            var validator = new Mock<IUserValidator<TUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<TUser>>();
            pwdValidators.Add(new PasswordValidator<TUser>());
            var userManager = new Mock<UserManager<TUser>>(store.Object, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<TUser>>>().Object);

            
            validator.Setup(v => v.ValidateAsync(userManager.Object, It.IsAny<TUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
            return userManager;
        }

        public static UserManager<TUser> TestUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        {
            store = store ?? new Mock<IUserStore<TUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;
            options.Setup(o => o.Value).Returns(idOptions);
            var userValidators = new List<IUserValidator<TUser>>();
            var validator = new Mock<IUserValidator<TUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<TUser>>();
            pwdValidators.Add(new PasswordValidator<TUser>());
            var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<TUser>>>().Object);
            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
            return userManager;
        }
    }
}
