using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AgencyBanking_BLL.util;
using AgencyBanking_BLL.ViewModel;
using Microsoft.AspNet.Identity;
using VisibilityPortal_BLL.Models;
using VisibilityPortal_BLL.Models.ASP_Identity;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;

namespace AgencyBanking_BLL.Controllers
{
    [Authorize(Roles= "SuperAdmin,SystemAdmin")]
    
  public  class UserController: Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public UserController()
        {
        }

        public UserController(
            ApplicationUserManager userManager, ApplicationSignInManager signInManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }
        public ApplicationRoleManager RoleManager
        {
            get => _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            private set => _roleManager = value;
        }


        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Registers user 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(UserModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                //Generate Random Password
                string password = Membership.GeneratePassword(12, 1);
                //TODO: create user with the ModuleID assigned by getting the ModuleId of the already signed in person
                IdentityResult result = await UserManager.CreateAsync(user,password);
                //send email
                if (result.Succeeded)
                {
                    
                    var userID = UserManager.FindByEmailAsync(model.Email).Result.Id;
                   await UserManager.AddToRoleAsync(userID, model.Role);
                    string emailConfirmationCode = await
                        UserManager.GenerateEmailConfirmationTokenAsync(userID);
                    //Generate a random password
                    string passwordSetCode = await UserManager.GeneratePasswordResetTokenAsync(userID);
                    //Generate Callback email
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userID, emailConfirmationCode = emailConfirmationCode, passwordSetCode = passwordSetCode }, protocol: Request.Url.Scheme);
                    //Send email Notification
                    await UserManager.SendEmailAsync(userID, "Confirm Your Account",
                        $"Please confirm your account by clicking <a href=\"{callbackUrl}\">here</a>");
                    return Content(new StatusMessage
                    {
                        Code = "200",
                        Message = "Success"
                    }.toJson());
                }
                else
                {
                    return Content(new StatusMessage
                    {
                        Code = "500",
                        Message = "User Already Exists"
                    }.toJson());
                }

            }
            else
            {
                var status = new HttpStatusCodeResult(500);
                return status;
            }
        }

    }
}
