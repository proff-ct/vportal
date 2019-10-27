using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Moq;
using NUnit.Framework;
using VisibilityPortal_BLL.Models;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;
using System;
using VisibilityPortal_BLL.InitialSetup.SetupDefaults;

namespace VisibilityPortal_BLL.Tests
{
  namespace InitSetupBLL.BLL_Functions
  {
    [TestFixture]
    public class VerifySuperUserPassphraseAsync
    {
      private InitialSetupBLL _initialSetupBLL = new InitialSetupBLL();

      public string passPhrase = "MyPassPhrase";
      public ApplicationRoleManager RoleManager;

      [OneTimeSetUp]
      public void InitTest()
      {
        Mock<IRoleStore<ApplicationRole>> mockRoleStore = new Mock<IRoleStore<ApplicationRole>>();
        mockRoleStore
          .Setup(r => r.FindByNameAsync(PortalUserRoles.SystemRoles.SuperAdmin.ToString()))
          .ReturnsAsync(new ApplicationRole()
          {
            Name = PortalUserRoles.SystemRoles.SuperAdmin.ToString(),
            SetupPassPhrase = passPhrase
          });

        RoleManager = new ApplicationRoleManager(mockRoleStore.Object);
      }
      [OneTimeTearDown]
      public void TestCleanup()
      {
        RoleManager.Dispose();
      }
      [Test]
      public void Returns_true_if_passphrase_correct()
      {
        bool result = Task.Run(() =>
          _initialSetupBLL.VerifySuperUserPassphraseAsync(RoleManager, passPhrase)).Result;

        Assert.IsInstanceOf<bool>(result, "Boolean not returned!!!");
        Assert.IsTrue(result, "True not returned for correct passphrase!");
      }
      [Test]
      public void Returns_false_if_passphrase_not_correct()
      {
        bool result = Task.Run(() =>
         _initialSetupBLL.VerifySuperUserPassphraseAsync(RoleManager, "Incorrect")).Result;

        Assert.IsInstanceOf<bool>(result, "Boolean not returned!!!");
        Assert.IsFalse(result, "False not returned for incorrect passphrase!!");
      }
      [Test]
      public void Returns_false_if_empty_string_passphrase_supplied()
      {
        bool result = Task.Run(() =>
        _initialSetupBLL.VerifySuperUserPassphraseAsync(RoleManager, "")).Result;

        Assert.IsInstanceOf<bool>(result, "Boolean not returned!!!");
        Assert.IsFalse(result, "False not returned for empty string passphrase!!");
      }
      [Test]
      public void Returns_false_if_passphrase_case_not_equal_to_expected_passphrase_case()
      {
        bool result = Task.Run(() =>
        _initialSetupBLL.VerifySuperUserPassphraseAsync(RoleManager, passPhrase.ToLower())).Result;

        Assert.IsInstanceOf<bool>(result, "Boolean not returned!!!");
        Assert.IsFalse(result, "False not returned for empty string passphrase!!");
      }
    }

    [TestFixture]
    public class GenerateSuperUserSetupPassPhrase
    {
      private InitialSetupBLL _initialSetupBLL = new InitialSetupBLL();

      public ApplicationRoleManager RoleManager;
      public string plainTextPassPhrase = "Pass phrase to encrypt";
      private string _encryptionKey = "";

      [OneTimeSetUp]
      public void InitTest()
      {
        var mockRoleStore = new Mock<IRoleStore<ApplicationRole>>();
        mockRoleStore
          .Setup(r => r.FindByNameAsync(PortalUserRoles.SystemRoles.SuperAdmin.ToString()))
          .ReturnsAsync(new ApplicationRole()
          {
            Name = PortalUserRoles.SystemRoles.SuperAdmin.ToString(),
            SetupPassPhrase = "NOT SET"
          });

        RoleManager = new ApplicationRoleManager(mockRoleStore.Object);
      }
      [OneTimeTearDown]
      public void TestCleanup()
      {
        RoleManager.Dispose();
      }
      [Test]
      public void Generates_an_encrytped_passphrase_string_together_with_encrpytion_key()
      {
        var output = _initialSetupBLL.GenerateSuperUserSetupPassPhrase(
          plainTextPassPhrase, out _encryptionKey);

        Assert.IsInstanceOf<string>(output, "String datatype not returned!!");
        Assert.True(!string.IsNullOrEmpty(_encryptionKey), "Encryption key not modified!");
      }
    }
    [TestFixture]
    public class SetSuperUserPassPhraseAsync
    {
      private InitialSetupBLL _initialSetupBLL = new InitialSetupBLL();

      public ApplicationRoleManager RoleManager;
      public string plainTextPassPhrase = "Pass phrase to encrypt";

      [OneTimeSetUp]
      public void InitTest()
      {
        var mockRoleStore = new Mock<IRoleStore<ApplicationRole>>();
        mockRoleStore
          .Setup(r => r.FindByNameAsync(PortalUserRoles.SystemRoles.SuperAdmin.ToString()))
          .ReturnsAsync(new ApplicationRole()
          {
            Name = PortalUserRoles.SystemRoles.SuperAdmin.ToString(),
            SetupPassPhrase = "NOT SET",
            SetupKey = "NOT DEFINED"
          });

        RoleManager = new ApplicationRoleManager(mockRoleStore.Object);
      }
      [OneTimeTearDown]
      public void TestCleanup()
      {
        RoleManager.Dispose();
      }

      [Test]
      public void Sets_the_passphrase_and_encryption_key_for_superuser_role()
      {
        string superUserRoleName = PortalUserRoles.SystemRoles.SuperAdmin.ToString();
        // get passphrase and encryption key before calling the function
        ApplicationRole roleToUpdate = RoleManager.FindByName(superUserRoleName);
        string setupKeyBeforeUpdate = roleToUpdate.SetupKey;
        string setupPassphraseBeforeUpdate = roleToUpdate.SetupPassPhrase;

        var result = Task.Run(() =>
         _initialSetupBLL.SetSuperUserPassPhraseAsync(RoleManager, plainTextPassPhrase)).Result;

        Assert.AreNotEqual(setupPassphraseBeforeUpdate, roleToUpdate.SetupPassPhrase, "Passphrase!");
        Assert.AreNotEqual(setupKeyBeforeUpdate, roleToUpdate.SetupKey, "SetupKey!!");
      }
      [Test]
      public void Returns_error_message_if_empty_passphrase_supplied()
      {
        var result = Task.Run(() =>
         _initialSetupBLL.SetSuperUserPassPhraseAsync(RoleManager, "")).Result;

        Assert.IsInstanceOf<string>(result);
        Assert.IsTrue(result.Contains("Error"), "Error not returned");
      }
      [Test]
      public void Returns_error_message_if_passphrase_string_length_less_than_five()
      {
        var result = Task.Run(() =>
         _initialSetupBLL.SetSuperUserPassPhraseAsync(RoleManager, 1234.ToString())).Result;

        Assert.IsInstanceOf<string>(result);
        Assert.IsTrue(result.Contains("Error"), "Error not returned");
      }
    }
    
    [TestFixture]
    public class CheckForDefaultSuperUserPassphrase
    {
      private InitialSetupBLL _initialSetupBLL = new InitialSetupBLL();

      public ApplicationRoleManager RoleManager;

      [OneTimeSetUp]
      public void InitTest()
      {
        var mockRoleStore = new Mock<IRoleStore<ApplicationRole>>();
        mockRoleStore
          .Setup(r => r.FindByNameAsync(PortalUserRoles.SystemRoles.SuperAdmin.ToString()))
          .ReturnsAsync(new ApplicationRole()
          {
            Name = PortalUserRoles.SystemRoles.SuperAdmin.ToString(),
            SetupPassPhrase = ApplicationRoleDefaults.PASSPHRASE_DEFAULT
          });

        RoleManager = new ApplicationRoleManager(mockRoleStore.Object);
      }
      [OneTimeTearDown]
      public void TestCleanup()
      {
        RoleManager.Dispose();
      }
      [Test]
      public void Returns_true_if_passphrase_has_default_value()
      {
        var output = _initialSetupBLL.CheckForDefaultSuperUserPassphrase(RoleManager);

        Assert.IsInstanceOf<bool>(output);
        Assert.IsTrue(output, "True not returned!!");
      }
      [Test]
      public void Returns_false_if_passphrase_does_not_have_default_value()
      {
        // modify the pass phrase first
        ApplicationRole superUserRole = RoleManager.FindByName(PortalUserRoles.SystemRoles.SuperAdmin.ToString());
        superUserRole.SetupPassPhrase = "random passphrase";
        RoleManager.Update(superUserRole);
        var output = _initialSetupBLL.CheckForDefaultSuperUserPassphrase(RoleManager);

        Assert.IsInstanceOf<bool>(output);
        Assert.IsFalse(output, "False not returned!!");
      }
    }

  }
}
