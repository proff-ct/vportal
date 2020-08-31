using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PagedList;
using Utilities.PortalApplicationParams;
using VisibilityPortal_BLL.CustomFilters;
using VisibilityPortal_BLL.Models;
using VisibilityPortal_BLL.Models.ASP_Identity;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;
using VisibilityPortal_BLL.Models.ViewModels;
using VisibilityPortal_BLL.Utilities.PortalDefaults;
using VisibilityPortal_DAL;
using VisibilityPortal_Dataspecs.Models;

namespace VisibilityPortal_BLL.Controllers
{
  [Authorize]
  public class AccountController : Controller
  {
    private ApplicationSignInManager _signInManager;
    private ApplicationUserManager _userManager;
    private ApplicationRoleManager _roleManager;
    private IBL_PortalModule _portalModuleBLL = new PortalModuleBLL();
    private PortalUserRoleBLL _portalUserRoleBLL = new PortalUserRoleBLL();
    private CoretecClientBLL _coretecClientBLL = new CoretecClientBLL();
    private SaccoBLL _saccoBLL = new SaccoBLL();

    public AccountController()
    {
    }

    public AccountController(
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

    //
    // GET: /Account/Login
    [AllowAnonymous]
    public ActionResult Login(string returnUrl)
    {
      ViewBag.ReturnUrl = returnUrl;
      return View();
    }

    //
    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      // Do not allow log in of users whose emails are not confirmed
      ApplicationUser user = UserManager.Find(model.Email, model.Password);
      if (user == null)
      {
        ModelState.AddModelError("", "No user found matching supplied credentials");
        return View(model);
      }
      if (!await UserManager.IsEmailConfirmedAsync(user.Id))
      {
        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        string callbackUrl = Url.Action(
          "ResendConfirmEmail", "Account", new { userId = user.Id },
          protocol: Request.Url.Scheme);
        ViewBag.EmailConfirmationLink = callbackUrl;

        return View("CheckEmailForConfirmationLink");
      }
      // This doesn't count login failures towards account lockout
      // To enable password failures to trigger account lockout, change to shouldLockout: true
      SignInStatus result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
      switch (result)
      {
        case SignInStatus.Success:
          // Check if user is using the default password and redirect them to reset the password
          if (UserManager.CheckPassword(user, ApplicationUserDefaults.PASSWORD_DEFAULT))
          {
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            return RedirectToAction("ResetPassword", "Account", new { userId = user.Id, code = code });
          }
          // initialize the activeUserParams session object
          ActiveUserParams activeUserParams = new ActiveUserParams
          {
            ClientCorporateNo = user.ClientCorporateNo,
            ClientModuleId = string.Empty, // will get set when the user selects a module on login
            Roles = new List<ActiveUserParams.UserRoles>()
          };
          Session["ActiveUserParams"] = activeUserParams;
          List<ActiveUserParams.UserRoles> userRoles = new List<ActiveUserParams.UserRoles>();

          // redirect the user to appropriate module
          if (user.PortalRoles.Count() == 1)
          {

            PortalRole pr = user.PortalRoles.Single();
            PortalModuleForClient clientModule = _coretecClientBLL.GetPortalModuleForClient(
             pr.ClientModuleId);
            
            //
            if (!Mapper.Map<PortalUserRole>(pr).IsEnabled)
            {
              return RedirectToAction("NoRolesEnabled");
            }
            
            // Set the ClientModuleId for the ActiveUserParams object
            activeUserParams.ClientModuleId = pr.ClientModuleId;
            Session["ActiveUserParams"] = activeUserParams;

            // Role is enabled. Therefore, proceed normally
            if (clientModule.PortalModuleName == PortalModule.AgencyBankingModule.moduleName)
            {

              return RedirectToRoute(
                PortalModule.AgencyBankingModule.defaultRoute,
                (RouteTable.Routes[PortalModule.AgencyBankingModule.defaultRoute] as Route).Defaults);
            }
            else if (clientModule.PortalModuleName == PortalModule.MSaccoModule.moduleName)
            {

              return RedirectToRoute(
                PortalModule.MSaccoModule.defaultRoute,
                (RouteTable.Routes[PortalModule.MSaccoModule.defaultRoute] as Route).Defaults);
            }
            else if (clientModule.PortalModuleName == PortalModule.CallCenterModule.moduleName)
            {
              return RedirectToRoute(
                PortalModule.CallCenterModule.defaultRoute,
                (RouteTable.Routes[PortalModule.CallCenterModule.defaultRoute] as Route).Defaults);
            }
            else
            {
              ViewBag.ModuleName = clientModule.PortalModuleName;
              return View("ModuleNotFound");
            }

          }
          else if (user.PortalRoles.Count() > 1)
          {
            try
            {
              List<ModuleUrl> moduleUrls = new List<ModuleUrl>();
              user.PortalRoles
                .Where(pr => Mapper.Map<PortalUserRole>(pr).IsEnabled)
                .ToList()
                .ForEach(pr =>
              {
                PortalModuleForClient clientModule = _coretecClientBLL.GetPortalModuleForClient(
                  pr.ClientModuleId);
                moduleUrls.Add(new ModuleUrl
                {
                  ModuleName = clientModule.PortalModuleName,
                  DefaultRoute = _portalModuleBLL.GetDefaultRouteForModule(clientModule.PortalModuleName)
                });
              });
              if (moduleUrls.Count == 0) return RedirectToAction("NoRolesEnabled");
              Session["ClientModuleUrls"] = moduleUrls.AsEnumerable();
              return RedirectToAction("Index", "Home");
            }
            catch (ArgumentException ex)
            {
              ViewBag.ErrorMessage = ex.Message;
              return View("UserModuleError");
            }
          }
          // leaving this here for the time being
          return RedirectToLocal(returnUrl);
        case SignInStatus.LockedOut:
          return View("Lockout");
        case SignInStatus.RequiresVerification:
          return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        case SignInStatus.Failure:
        default:
          ModelState.AddModelError("", "Invalid login attempt.");
          return View(model);
      }
    }

    //
    // GET: /Account/VerifyCode
    [AllowAnonymous]
    public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
    {
      // Require that the user has already logged in via username/password or external login
      if (!await SignInManager.HasBeenVerifiedAsync())
      {
        return View("Error");
      }
      return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
    }

    //
    // POST: /Account/VerifyCode
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // The following code protects for brute force attacks against the two factor codes. 
      // If a user enters incorrect codes for a specified amount of time then the user account 
      // will be locked out for a specified amount of time. 
      // You can configure the account lockout settings in IdentityConfig
      SignInStatus result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
      switch (result)
      {
        case SignInStatus.Success:
          return RedirectToLocal(model.ReturnUrl);
        case SignInStatus.LockedOut:
          return View("Lockout");
        case SignInStatus.Failure:
        default:
          ModelState.AddModelError("", "Invalid code.");
          return View(model);
      }
    }

    //
    // GET: /Account/Register
    [AllowAnonymous]
    public ActionResult Register()
    {
      return View();
    }

    //
    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        IdentityResult result = await UserManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

          // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
          // Send an email with this link
          // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
          // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
          // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

          return RedirectToAction("Index", "Home");
        }
        AddErrors(result);
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    //
    // GET: /Account/CreateSuperAdmin
    [AllowAnonymous]
    public ActionResult CreateSuperAdmin()
    {
      return View();
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateSuperAdmin(RegisterSuperUserViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      // verify the pass phrase

      bool isValidPassPhrase = await VerifyPassPhraseAsync(RoleManager, model.setupPassPhrase);
      if (!isValidPassPhrase)
      {
        ModelState.AddModelError("setupPassPhrase", "Passphrase verification failed");
        return View(model);
      }

      ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
      IdentityResult result = await UserManager.CreateAsync(user, model.Password);
      if (result.Succeeded)
      {
        // add the superadmin role to the user
        user.ClientCorporateNo = CoreTecOrganisation.CorporateNo;
        UserManager.AddToRole(user.Id, PortalUserRoles.SystemRoles.SuperAdmin.ToString());
        UserManager.Update(user);
        try
        {
          await SendActivationEmailAsync(user);
          return RedirectToAction("Login", "Account");
        }
        catch (SmtpException ex)
        {
          ViewBag.ErrorMessage = ex.Message;
          return View("ErrorSendEmail");
        }
      }
      else
      {
        AddErrors(result);
        return View(model);
      }

    }
    //
    // GET: /Account/ConfirmEmail
    /// <summary>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="emailConfirmationCode"></param>
    /// <param name="passwordSetCode"></param>
    /// <returns></returns>
    [AllowAnonymous]

    public async Task<ActionResult> ConfirmEmail(string userId, string emailConfirmationCode, string passwordSetCode = null)
    {
      if (userId == null || emailConfirmationCode == null)
      {
        return View("Error");
      }
      IdentityResult result = await UserManager.ConfirmEmailAsync(userId, emailConfirmationCode);

      if (result.Succeeded)
      {
        ApplicationUser user = UserManager.FindById(userId);
        user.DateEmailConfirmed = DateTime.Now;
        UserManager.Update(user);
      }


      if (result.Succeeded && !string.IsNullOrEmpty(passwordSetCode))
      {
        return RedirectToAction("ResetPassword", "Account", new { userId = userId, code = passwordSetCode, firstPassword = true });
      }

      return View(result.Succeeded ? "ConfirmEmail" : "Error");
    }

    //
    // GET: /Account/ResendConfirmEmail
    [AllowAnonymous]
    public async Task<ActionResult> ResendConfirmEmail(string userId)
    {
      ApplicationUser user = UserManager.FindById(userId);
      if (user == null)
      {
        return View("~/Views/Shared/Error"); // use a 404 page instead
      }

      string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
      string callbackUrl = Url.Action(
        "ConfirmEmail", "Account", new { userId = user.Id, emailConfirmationCode = code },
        protocol: Request.Url.Scheme);
      IdentityMessage message = new IdentityMessage
      {
        Destination = user.Email,
        Subject = "Visibility Portal: Activate your account",
        Body = $"Please activate your account by clicking <a href ='{callbackUrl}'>here</a>",
      };
      try
      {
        await UserManager.EmailService.SendAsync(message);
        return RedirectToAction("Login", "Account");
      }
      catch (SmtpException ex)
      {
        ViewBag.ErrorMessage = ex.Message;
        return View("ErrorSendEmail");
      }
    }
    //
    // GET: /Account/ForgotPassword
    [AllowAnonymous]
    public ActionResult ForgotPassword()
    {
      return View();
    }

    //
    // POST: /Account/ForgotPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        ApplicationUser user = await UserManager.FindByNameAsync(model.Email);
        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        {
          // Don't reveal that the user does not exist or is not confirmed
          return View("ForgotPasswordConfirmation");
        }

        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        // Send an email with this link
        string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        var callbackUrl = Url.Action(
          "ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

        IdentityMessage emailMessage = new IdentityMessage
        {
          Destination = user.Email,
          Subject = "CoreTec Visibility Portal: Reset your password",
          Body = $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>"
        };
        await UserManager.EmailService.SendAsync(emailMessage);
        //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        return RedirectToAction("ForgotPasswordConfirmation", "Account");
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    //
    // GET: /Account/ForgotPasswordConfirmation
    [AllowAnonymous]
    public ActionResult ForgotPasswordConfirmation()
    {
      return View();
    }

    //
    // GET: /Account/ResetPassword
    [AllowAnonymous]
    public ActionResult ResetPassword(string code)
    {
      return code == null ? View("Error") : View();
    }

    //
    // POST: /Account/ResetPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      ApplicationUser user = await UserManager.FindByNameAsync(model.Email);
      if (user == null)
      {
        // Don't reveal that the user does not exist
        return View("ResetPasswordFail");
      }
      IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
      if (result.Succeeded)
      {
        if (!UserManager.CheckPassword(user, ApplicationUserDefaults.PASSWORD_DEFAULT))
        {
          user.IsDefaultPassword = false;
          UserManager.Update(user);
        }
        return RedirectToAction("ResetPasswordConfirmation", "Account");
      }
      AddErrors(result);
      return View();
    }

    //
    // GET: /Account/ResetPasswordConfirmation
    [AllowAnonymous]
    public ActionResult ResetPasswordConfirmation()
    {
      return View();
    }

    //
    // POST: /Account/ExternalLogin
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult ExternalLogin(string provider, string returnUrl)
    {
      // Request a redirect to the external login provider
      return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
    }

    //
    // GET: /Account/SendCode
    [AllowAnonymous]
    public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
    {
      string userId = await SignInManager.GetVerifiedUserIdAsync();
      if (userId == null)
      {
        return View("Error");
      }
      System.Collections.Generic.IList<string> userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
      System.Collections.Generic.List<SelectListItem> factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
      return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
    }

    //
    // POST: /Account/SendCode
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SendCode(SendCodeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      // Generate the token and send it
      if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
      {
        return View("Error");
      }
      return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
    }

    //
    // GET: /Account/ExternalLoginCallback
    [AllowAnonymous]
    public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
    {
      ExternalLoginInfo loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
      if (loginInfo == null)
      {
        return RedirectToAction("Login");
      }

      // Sign in the user with this external login provider if the user already has a login
      SignInStatus result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
      switch (result)
      {
        case SignInStatus.Success:
          return RedirectToLocal(returnUrl);
        case SignInStatus.LockedOut:
          return View("Lockout");
        case SignInStatus.RequiresVerification:
          return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        case SignInStatus.Failure:
        default:
          // If the user does not have an account, then prompt the user to create an account
          ViewBag.ReturnUrl = returnUrl;
          ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
          return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
      }
    }

    //
    // POST: /Account/ExternalLoginConfirmation
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
    {
      if (User.Identity.IsAuthenticated)
      {
        return RedirectToAction("Index", "Manage");
      }

      if (ModelState.IsValid)
      {
        // Get the information about the user from the external login provider
        ExternalLoginInfo info = await AuthenticationManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
          return View("ExternalLoginFailure");
        }
        ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        IdentityResult result = await UserManager.CreateAsync(user);
        if (result.Succeeded)
        {
          result = await UserManager.AddLoginAsync(user.Id, info.Login);
          if (result.Succeeded)
          {
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            return RedirectToLocal(returnUrl);
          }
        }
        AddErrors(result);
      }

      ViewBag.ReturnUrl = returnUrl;
      return View(model);
    }

    //
    // GET: /Account/SignOut
    public ActionResult SignOut()
    {
      Session.Clear();
      Session.Abandon();
      AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
      return RedirectToAction("Index", "Home");
    }

    //
    // POST: /Account/LogOff
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LogOff()
    {
      Session.Clear();
      Session.Abandon();
      AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
      return RedirectToAction("Index", "Home");
    }

    //
    // GET: /Account/ExternalLoginFailure
    [AllowAnonymous]
    public ActionResult ExternalLoginFailure()
    {
      return View();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_userManager != null)
        {
          _userManager.Dispose();
          _userManager = null;
        }

        if (_signInManager != null)
        {
          _signInManager.Dispose();
          _signInManager = null;
        }
      }

      base.Dispose(disposing);
    }

    #region Portal
    //
    // GET: /Account/Index
    [Authorize]
    [RequireSuperOrSystemAdmin]
    public ActionResult Index()
    {
      return View();
    }
    //
    // GET: /Account/Index
    [Authorize]
    public ActionResult NoRolesEnabled()
    {
      return View();
    }
    // GET: /Account/AddOrUpdate
    [Authorize]
    [RequireSuperOrSystemAdmin]
    public ActionResult AddNew()
    {
      SetApplicationRoleList();
      SetClientPortalModuleParamsForUser();
      return View();
    }
    //
    // POST: /Account/AddOrUpdate
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    //public async Task<ActionResult> AddOrUpdate(RegisterViewModel model)
    //{
    //  if (ModelState.IsValid)
    //  {
    //    ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
    //    IdentityResult result = await UserManager.CreateAsync(user, model.Password);
    //    if (result.Succeeded)
    //    {
    //      await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

    //      // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
    //      // Send an email with this link
    //      // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
    //      // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
    //      // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

    //      return RedirectToAction("Index", "Home");
    //    }
    //    AddErrors(result);
    //  }

    //  // If we got this far, something failed, redisplay form
    //  return View(model);
    //}
    public async Task<ActionResult> AddNew(ClientUserViewModel clientUser)
    {
      if (!ModelState.IsValid)
      {
        SetApplicationRoleList();
        SetClientPortalModuleParamsForUser();
        return View(clientUser);
      }
      // validate that we have a valid role
      ApplicationRole roleToAssign = RoleManager.FindById(clientUser.RoleId);
      if (roleToAssign == null)
      {
        ModelState.AddModelError("role", "Role DOES NOT EXIST!");
        SetApplicationRoleList();
        SetClientPortalModuleParamsForUser();
        return View(clientUser);
      }

      // check if user is new or existing
      ApplicationUser existingUser = UserManager.FindByEmail(clientUser.Email);
      if (existingUser != null)
      {
        // user already exists. By design, a given email address can only be used for one client

        // check that the CorporateNo of existing user matches the one selected else return error
        if (existingUser.ClientCorporateNo != clientUser.ClientCorporateNo)
        {
          ModelState.AddModelError("Email", "Email already registered in a different organisation");
          SetApplicationRoleList();
          SetClientPortalModuleParamsForUser();
          return View(clientUser);
        }

        // check that we are not assigning an existing role for a given module
        existingUser.PortalRoles = Mapper.Map<IEnumerable<PortalRole>>(
          _portalUserRoleBLL.GetPortalUserRoleListForUser(existingUser.Id));
        List<PortalRole> listExistingUserPortalRoles = new List<PortalRole>();

        // By design, a user can have only one role per client module.
        // So, if the user already has any other role for the client module, do not create
        if (existingUser.PortalRoles.ToList().Exists(r => r.ClientModuleId == clientUser.ClientModuleId))
        {
          ModelState.AddModelError("", "User already has a role for the selected module");
          SetApplicationRoleList();
          SetClientPortalModuleParamsForUser();
          return View(clientUser);
        }
        // role does not exist. Create the role
        UserManager.AddToRole(existingUser.Id, roleToAssign.Name);
        PortalRole portalRole = new PortalRole(
        clientUser.ClientModuleId, roleToAssign.Id, roleToAssign.Name);
        listExistingUserPortalRoles.Add(portalRole);
        existingUser.PortalRoles = listExistingUserPortalRoles.AsEnumerable();

        PortalUserRole portalUserRole = Mapper.Map<PortalUserRole>(portalRole);
        portalUserRole.UserId = existingUser.Id;
        portalUserRole.CreatedBy = User.Identity.GetUserName();
        UserManager.Update(existingUser);
        _portalUserRoleBLL.Save(portalUserRole, ModelOperation.AddNew);



        return RedirectToAction("Index", "Account");
      }

      // Create user
      ApplicationUser user = new ApplicationUser
      {
        ClientCorporateNo = clientUser.ClientCorporateNo,
        FirstName = clientUser.FirstName,
        LastName = clientUser.LastName,
        UserName = clientUser.Email,
        Email = clientUser.Email,
        IsDefaultPassword = true,
        CreatedBy = User.Identity.GetUserName()
      };
      IdentityResult result = await UserManager.CreateAsync(user, ApplicationUserDefaults.PASSWORD_DEFAULT);
      if (result.Succeeded)
      {
        UserManager.AddToRole(user.Id, roleToAssign.Name);
        List<PortalRole> listPortalRolesForUser = new List<PortalRole>();
        PortalRole portalRole = new PortalRole(
          clientUser.ClientModuleId, roleToAssign.Id, roleToAssign.Name);
        listPortalRolesForUser.Add(portalRole);
        user.PortalRoles = listPortalRolesForUser.AsEnumerable();

        PortalUserRole portalUserRole = Mapper.Map<PortalUserRole>(portalRole);
        portalUserRole.UserId = user.Id;
        portalUserRole.CreatedBy = User.Identity.GetUserName();

        UserManager.Update(user);
        _portalUserRoleBLL.Save(portalUserRole, ModelOperation.AddNew);
        // send activation email and redirect to index
        try
        {
          await SendActivationEmailAsync(user);
          return RedirectToAction("Index", "Account");
        }
        catch (SmtpException ex)
        {
          ViewBag.ErrorMessage = ex.Message;
          return View("ErrorSendEmail");
        }
      }
      else
      {
        AddErrors(result);
        SetApplicationRoleList();
        SetClientPortalModuleParamsForUser();
        return View(clientUser);
      }
    }
    [HttpGet]
    [Authorize]
    [RequireSuperOrSystemAdmin]
    public ActionResult Edit(string email)
    {
      // locate user based on passed in email
      // if user not found display a 404
      // else display the edit page
      if (string.IsNullOrEmpty(email))
      {
        return View();
      }

      ApplicationUser user = UserManager.FindByEmail(email);
      if (user == null)
      {
        ViewBag.UnknownEmail = email;
        return View();
      }

      EditPortalUserViewModel editUserVM = new EditPortalUserViewModel
      {
        ClientCorporateNo = user.ClientCorporateNo,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        PortalRoles = Mapper.Map<IList<PortalUserRoleViewModel>>(
          _portalUserRoleBLL.GetPortalUserRoleListForUser(user.Id))
          .Select(r =>
          {
            r.Module = _coretecClientBLL.GetPortalModuleForClient(r.ClientModuleId).PortalModuleName;
            return r;
          })
          .ToList()
      };

      SetViewParamsForEditingUser(user);
      return View(editUserVM);
    }
    [HttpPost]
    [Authorize]
    [RequireSuperOrSystemAdmin]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditPortalUserViewModel editUserVM)
    {
      ApplicationUser userToEdit = UserManager.FindByEmail(editUserVM.Email);
      if(userToEdit == null)
      {
        ViewBag.UnknownEmail = editUserVM.Email;
        return View();
      }
      if (!ModelState.IsValid)
      {
        SetViewParamsForEditingUser(userToEdit);
        return View(editUserVM);
      }
      // save the user's particulars
      userToEdit.FirstName = editUserVM.FirstName;
      userToEdit.LastName = editUserVM.LastName;
      userToEdit.ModifiedBy = User.Identity.GetUserName();
      userToEdit.ModifiedOn = DateTime.Now.ToUniversalTime();
      UserManager.Update(userToEdit);

      // save the roles
      // the only thing expected to change at the moment is th IsEnabled property
      if(editUserVM.PortalRoles.Count > 0)
      {
        PortalUserRole roleToUpdate = null;
        editUserVM.PortalRoles.ToList().ForEach(r => {
          roleToUpdate = _portalUserRoleBLL.GetPortalUserRoleListForUser(userToEdit.Id)
          .SingleOrDefault(userRoles => (userRoles.AspRoleId.Equals(r.AspRoleId) && userRoles.ClientModuleId.Equals(r.ClientModuleId)));

          if (roleToUpdate == null) return;

          roleToUpdate.IsEnabled = r.IsEnabled;
          roleToUpdate.ModifiedBy = User.Identity.GetUserName();

          if (!_portalUserRoleBLL.Save(roleToUpdate, ModelOperation.Update))
          {
            ModelState.AddModelError("", $"Unable to save User's Role({r.AspRoleName}) in db");
          };
        });
      }
      if (!ModelState.IsValid)
      {
        return View(editUserVM);
      }
      else
      {
        return RedirectToAction("Edit", new { email = userToEdit.Email });
      }
      // return to edit 
    }
    [HttpGet]
    [Authorize]
    [RequireSuperOrSystemAdmin]
    public ActionResult AddRoleToUser(string email)
    {
      // locate user based on passed in email
      // if user not found display a 404
      // else display the edit page
      if (string.IsNullOrEmpty(email))
      {
        return View();
      }

      ApplicationUser user = UserManager.FindByEmail(email);
      if (user == null)
      {
        ViewBag.UnknownEmail = email;
        return View();
      }
            
      if (User.IsInRole(PortalUserRoles.SystemRoles.SuperAdmin.ToString()))
      {
        ViewBag.UserClientName = user.ClientCorporateNo.Equals(CoreTecOrganisation.CorporateNo) ?
          CoreTecOrganisation.CorporateName : _saccoBLL.GetSaccoByUniqueParam(user.ClientCorporateNo).saccoName_1;
      }
      else if (User.IsInRole(PortalUserRoles.SystemRoles.SystemAdmin.ToString()))
      {
        SetClientPortalModuleParamsForUser();
      }

      AddUserRoleViewModel addUserRoleVM = new AddUserRoleViewModel
      {
        ClientModuleId = string.Empty,
        ClientCorporateNo = user.ClientCorporateNo,
        Email = user.Email,
      };

      ViewBag.ClientModuleList = Mapper.Map<List<CoretecClientModuleViewModel>>(
        _coretecClientBLL.GetAllPortalModulesForClient(addUserRoleVM.ClientCorporateNo).ToList());

      return View(addUserRoleVM);
    }
    [HttpPost]
    [Authorize]
    [RequireSuperOrSystemAdmin]
    [ValidateAntiForgeryToken]
    public ActionResult AddRoleToUser(AddUserRoleViewModel addUserRoleVM)
    {
      
      ApplicationUser userToEdit = UserManager.FindByEmail(addUserRoleVM.Email);
      if (userToEdit == null)
      {
        ViewBag.UnknownEmail = addUserRoleVM.Email;
        return View();
      }

      if (!ModelState.IsValid)
      {
        SetViewParamsForEditingUser(userToEdit);
        ViewBag.ClientModuleList = Mapper.Map<List<CoretecClientModuleViewModel>>(
        _coretecClientBLL.GetAllPortalModulesForClient(addUserRoleVM.ClientCorporateNo).ToList());
        return View(addUserRoleVM);
      }
      // validate that we have a valid role
      ApplicationRole roleToAssign = RoleManager.FindById(addUserRoleVM.RoleId);
      if (roleToAssign == null)
      {
        ModelState.AddModelError("role", "Role DOES NOT EXIST!");
        ViewBag.ClientModuleList = Mapper.Map<List<CoretecClientModuleViewModel>>(
        _coretecClientBLL.GetAllPortalModulesForClient(addUserRoleVM.ClientCorporateNo).ToList());
        SetClientPortalModuleParamsForUser();

        return View(addUserRoleVM);
      }
      // save the roles
      UserManager.AddToRole(userToEdit.Id, roleToAssign.Name);
      UserManager.Update(userToEdit);

      PortalRole portalRole = new PortalRole(
        addUserRoleVM.ClientModuleId, roleToAssign.Id, roleToAssign.Name);
      PortalUserRole portalUserRole = Mapper.Map<PortalUserRole>(portalRole);
      portalUserRole.UserId = userToEdit.Id;
      portalUserRole.CreatedBy = User.Identity.GetUserName();
      
      _portalUserRoleBLL.Save(portalUserRole, ModelOperation.AddNew);

      // return to edit 
      return RedirectToAction("Edit", new { email = userToEdit.Email });
      
    }

    [HttpGet]
    [Authorize]
    public ActionResult GetPortalUsers(
      int page, int size, string clientCorporateNo, string clientModuleId = null)
    {
      IPagedList<IEnumerable<ApplicationUserViewModel>> pagedUserList = null;
      List<ApplicationUserViewModel> clientUsers = new List<ApplicationUserViewModel>();
      List<CoretecClientWithUsers> clientsWithUsers = new List<CoretecClientWithUsers>();
      clientsWithUsers = Mapper.Map<List<CoretecClientWithUsers>>(
          _coretecClientBLL.GetRegisteredClients());

      if (string.IsNullOrEmpty(clientCorporateNo))
      {
        // load all users
        clientsWithUsers.ForEach(c =>
        {
          c.Users = Mapper.Map<IEnumerable<ApplicationUserViewModel>>(
            UserManager.Users.Where(u => u.ClientCorporateNo == c.corporateNo).ToList());

          if (c.Users.Count() > 0)
          {
            c.Users = c.Users
            .Select(user =>
            {
              user.ClientName = (c.corporateNo == CoreTecOrganisation.CorporateNo) ?
                CoreTecOrganisation.CorporateName : c.saccoName_1;
              return user;
            })
            .ToList();
          }
        });

        pagedUserList = clientsWithUsers.Where(c => c.Users.Count() > 0)
          .Select(c => c.Users)
          .ToPagedList(page, size);
        return Json(new
        {
          last_page = pagedUserList.PageCount, // last page in fetched recordset
          data = clientsWithUsers
          .Where(c => c.Users.Count() > 0 && pagedUserList.ToList().Contains(c.Users))
          .SelectMany(clients => clients.Users)
          .ToArray()
        }, JsonRequestBehavior.AllowGet);
      }
      else
      {
        // load users for specified client
        CoretecClientWithUsers client = clientsWithUsers
          .Where(c => c.corporateNo == clientCorporateNo).SingleOrDefault();
        if (client != null)
        {
          client.Users = Mapper.Map<IEnumerable<ApplicationUserViewModel>>(
              UserManager.Users.Where(u => u.ClientCorporateNo == client.corporateNo))
              .Select(user =>
              {
                user.ClientName = (client.corporateNo == CoreTecOrganisation.CorporateNo) ?
                CoreTecOrganisation.CorporateName : client.saccoName_1;

                return user;
              });
          if (client.Users.Count() > 0)
          {
            return Json(new
            {
              last_page = client.Users.ToPagedList(page, size).PageCount, // last page in recordset
              data = client.Users.ToPagedList(page, size).ToArray()
            }, JsonRequestBehavior.AllowGet);
          }
          else
          {
            return Json(new
            {
              last_page = 0, // last page in fetched recordset
              data = Array.Empty<IEnumerable<ApplicationUserViewModel>>()
            }, JsonRequestBehavior.AllowGet);
          }
        }
        else
        {
          return Json(new
          {
            last_page = 0, // last page in fetched recordset
            data = Array.Empty<IEnumerable<ApplicationUserViewModel>>()
          }, JsonRequestBehavior.AllowGet);
        }


      }
    }
    [HttpGet]
    [Authorize]
    public ActionResult GetAvailableRolesForClientUser(string email, string clientModuleId)
    {
      if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(clientModuleId)) return null;
      ApplicationUser user = UserManager.FindByEmail(email);
      if (user == null) return null;

      List<PortalUserRole> listRolesForUser = new List<PortalUserRole>();
      listRolesForUser = _portalUserRoleBLL.GetPortalUserRoleListForUser(user.Id).ToList();

      PortalUserRole roleForModule = listRolesForUser.SingleOrDefault(r => r.ClientModuleId == clientModuleId);

      if(roleForModule == null || !roleForModule.IsEnabled)
      {
        return Json(
          Mapper.Map<List<PortalApplicationRoleViewModel>>(RoleManager.Roles.ToList())
          .Where(r=> !r.Name.Equals(PortalUserRoles.SystemRoles.SuperAdmin.ToString()))
          .OrderBy(r => r.Name)
          .Select(r => new { r.Name, r.RoleId })
          .ToList(), JsonRequestBehavior.AllowGet);
      }
      else
      {
        return null;
      }

    }
    #endregion



    #region Helpers
    // Used for XSRF protection when adding external logins
    private const string XsrfKey = "XsrfId";

    private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

    private void AddErrors(IdentityResult result)
    {
      foreach (string error in result.Errors)
      {
        ModelState.AddModelError("", error);
      }
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }
      return RedirectToAction("Index", "Home");
    }

    private async Task SendActivationEmailAsync(ApplicationUser user)
    {

      string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

      string callbackUrl = Url.Action(
            "ConfirmEmail", "Account", new { userId = user.Id, emailConfirmationCode = code },
            protocol: Request.Url.Scheme);

      IdentityMessage _confirmationEmail = new IdentityMessage
      {
        Destination = user.Email,
        Subject = "CoreTec Visibility Portal: Activate your account",
        Body = $"Please activate your account by clicking <a href ='{callbackUrl}'>here</a>@" +
        $"Once you activate your account, login using password {ApplicationUserDefaults.PASSWORD_DEFAULT}@" +
        $"You will then change this password and set one of your own choosing.@" +
        $"@" +
        $"@" +
        $"Best regards," +
        $"" +
        $"NB: This is a system generated email. You do not need to reply to this message"
      };
      _confirmationEmail.Body = _confirmationEmail.Body.Replace("@", Environment.NewLine);
      await UserManager.EmailService.SendAsync(_confirmationEmail);
    }
    private async Task<bool> VerifyPassPhraseAsync(ApplicationRoleManager roleManager, string passPhrase)
    {
      InitialSetupBLL initialSetupBLL = new InitialSetupBLL();
      try
      {
        return await initialSetupBLL.VerifySuperUserPassphraseAsync(RoleManager, passPhrase);
      }
      catch (ArgumentException)
      {
        //TO-DO:
        // Log that an exception occurred
        return false;
      }
    }
    internal class ChallengeResult : HttpUnauthorizedResult
    {
      public ChallengeResult(string provider, string redirectUri)
          : this(provider, redirectUri, null)
      {
      }

      public ChallengeResult(string provider, string redirectUri, string userId)
      {
        LoginProvider = provider;
        RedirectUri = redirectUri;
        UserId = userId;
      }

      public string LoginProvider { get; set; }
      public string RedirectUri { get; set; }
      public string UserId { get; set; }

      public override void ExecuteResult(ControllerContext context)
      {
        AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = RedirectUri };
        if (UserId != null)
        {
          properties.Dictionary[XsrfKey] = UserId;
        }
        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
      }
    }

    private void SetApplicationRoleList()
    {
      //if (User.IsInRole(PortalUserRoles.SystemRoles.SuperAdmin.ToString()))
      //{
      //  ViewBag.ApplicationRoleViewModelList = Mapper.Map<List<PortalApplicationRoleViewModel>>(RoleManager.Roles.ToList());
      //}
      //else if (User.IsInRole(PortalUserRoles.SystemRoles.SystemAdmin.ToString()))
      //{
      //  ViewBag.ApplicationRoleViewModelList = Mapper.Map<List<PortalApplicationRoleViewModel>>(RoleManager
      //    .Roles
      //    .Where(r => r.Name != PortalUserRoles.SystemRoles.SuperAdmin.ToString())
      //    .ToList());
      //}
      ViewBag.ApplicationRoleViewModelList = Mapper.Map<List<PortalApplicationRoleViewModel>>(RoleManager
          .Roles
          .Where(r => r.Name != PortalUserRoles.SystemRoles.SuperAdmin.ToString())
          .ToList());
    }
    private void SetClientPortalModuleParamsForUser()
    {
      if (Session != null && Session["ActiveUserParams"] != null)
      {
        ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
      }
      else
      {
        RedirectToAction("Login");
      }
    }
    private void SetViewParamsForEditingUser(ApplicationUser userToEdit)
    {
      if (User.IsInRole(PortalUserRoles.SystemRoles.SuperAdmin.ToString()))
      {
        ViewBag.UserClientName = userToEdit.ClientCorporateNo.Equals(CoreTecOrganisation.CorporateNo) ?
          CoreTecOrganisation.CorporateName : _saccoBLL.GetSaccoByUniqueParam(userToEdit.ClientCorporateNo).saccoName_1;
      }
      else if (User.IsInRole(PortalUserRoles.SystemRoles.SystemAdmin.ToString()))
      {
        SetClientPortalModuleParamsForUser();
      }
    }
    #endregion
  }
}