using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig;

namespace VisibilityPortal_BLL.Controllers
{

  public class InitialSetupController : Controller
  {
    private ApplicationRoleManager _roleManager;
    private InitialSetupBLL _initialSetupBLL = new InitialSetupBLL();
    public ApplicationRoleManager RoleManager
    {
      get => _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
      private set => _roleManager = value;
    }

    public InitialSetupController()
    {

    }
    public InitialSetupController(ApplicationRoleManager roleManager)
    {
      RoleManager = roleManager;
    }

    //
    // GET: /InitialSetup/SetPassphrase
    [AllowAnonymous]
    public ActionResult SetPassphrase()
    {
      return View();
    }
    //
    // POST: /InitialSetup/SetPassphrase
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SetPassphrase(FormCollection form)
    {
      string passPhrase = form.GetValue("passPhrase").AttemptedValue;
      if (!string.IsNullOrEmpty(passPhrase))
      {
        string response = await _initialSetupBLL.SetSuperUserPassPhraseAsync(RoleManager, passPhrase);
        return View("PassphraseSuccess");
      }
      else
      {
        ModelState.AddModelError("", "Passphrase is null or empty");
        return View(form);
      }
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<bool> ValidatePassPhrase(string passPhrase)
    {
      try
      {
        //return Task.Run(() => _initialSetupBLL.VerifySuperUserPassphraseAsync(RoleManager, passPhrase)).Result;
        return await _initialSetupBLL.VerifySuperUserPassphraseAsync(RoleManager, passPhrase);
      }
      catch (ArgumentException)
      {
        //TO-DO:
        // Log that an exception occurred
        return false;
      }
    }
  }
}