using System;
using System.Threading.Tasks;
using MattAdoUtils.Encryption;
using Utilities.PortalSecurity;
using VisibilityPortal_BLL.InitialSetup.SetupDefaults;
using VisibilityPortal_BLL.Models;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;
using VisibilityPortal_Dataspecs.Models;

namespace VisibilityPortal_BLL
{
  public class InitialSetupBLL
  {
    public async Task<bool> VerifySuperUserPassphraseAsync(
      ApplicationRoleManager roleManager, string passPhrase)
    {
      if (string.IsNullOrEmpty(passPhrase)) return false;

      ApplicationRole superUserRole = await roleManager.FindByNameAsync(
        PortalUserRoles.SystemRoles.SuperAdmin.ToString());
      if (string.IsNullOrEmpty(superUserRole.SetupPassPhrase))
      {
        throw new ArgumentNullException("SetupPassPhrase","Passphrase not defined!");
      }
      else if(superUserRole.SetupPassPhrase == ApplicationRoleDefaults.PASSPHRASE_DEFAULT)
      {
        throw new ArgumentException("SetupPassPhrase", "Passphrase not configured!");
      }
      else if(
        superUserRole.SetupKey == ApplicationRoleDefaults.PASSKEY_DEFAULT 
        || string.IsNullOrEmpty(superUserRole.SetupKey))
      {
        throw new ArgumentException("SetupPassKey", "SetupKey not configured!");
      }
      
      return passPhrase == Encryption.Decrypt(superUserRole.SetupPassPhrase, superUserRole.SetupKey);
    }

    public string GenerateSuperUserSetupPassPhrase(string passPhrase, out string encryptionKey)
    {
      // get the pass phrase
      if (string.IsNullOrEmpty(passPhrase) || passPhrase.Length < 5)
      {
        encryptionKey = null;
        return null;
      }
      passPhrase = passPhrase.Trim();
      string salt = "{0}_{1}";
      string saltedPhrase = string.Format(
        salt, PortalSetupSecurityTokens.EncryptionSalt, passPhrase);
      // generate the keystring to encrypt the pass phrase
      encryptionKey = Encryption.GeneratePassKey(saltedPhrase);
      // encrypt the defined portal superuser's system role name
      return Encryption.Encrypt(passPhrase, encryptionKey);
    }
    public async Task<string> SetSuperUserPassPhraseAsync(
      ApplicationRoleManager roleManager, string passPhrase)
    {
      string superUserRoleName = PortalUserRoles.SystemRoles.SuperAdmin.ToString();
      string _encryptionKey = "";
      // set the pass phrase
      try
      {
        ApplicationRole superUserRole = await roleManager.FindByNameAsync(superUserRoleName);
        superUserRole.SetupPassPhrase = GenerateSuperUserSetupPassPhrase(
          ValidatePassphrase(passPhrase), out _encryptionKey);
        superUserRole.SetupKey = _encryptionKey;
        await roleManager.UpdateAsync(superUserRole);

        return "Setup passphrase succefully set";
      }
      catch (ArgumentNullException ex)
      {
        return $"Error occurred setting the passphrase: {ex.Message}";
      }
      catch (ArgumentException ex)
      {
        return $"Error occurred setting the passphrase: {ex.Message}";
      }
      catch (Exception ex)
      {
        return $"Error occurred setting the passphrase: {ex.Message}";
      }
    }

    public bool CheckForDefaultSuperUserPassphrase(ApplicationRoleManager roleManager)
    {
      string rolePassPhrase = roleManager
         .FindByNameAsync(PortalUserRoles.SystemRoles.SuperAdmin.ToString())
         .Result.SetupPassPhrase;

      return rolePassPhrase == ApplicationRoleDefaults.PASSPHRASE_DEFAULT;
    }
    private string ValidatePassphrase(string passPhrase)
    {
      if (string.IsNullOrEmpty(passPhrase))
      {
        throw new ArgumentNullException(
        "passPhrase", "Error! passPhrase is either null or empty!!");
      }

      passPhrase = passPhrase.Trim();
      if (passPhrase.Length < 5)
      {
        throw new ArgumentException("passPhrase is less than 5 characters", "passPhrase");
      }

      return passPhrase;
    }
  }

  namespace InitialSetup.SetupDefaults
  {
    public static class ApplicationRoleDefaults
    {
      public static string PASSPHRASE_DEFAULT => "NOT SET";
      public static string PASSKEY_DEFAULT => "NOT DEFINED";
    }

  }
}
