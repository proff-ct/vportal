using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_Dataspecs.Models;

namespace Utilities.PortalApplicationParams
{
  public class ActiveUserParams : IActiveUserParams
  {
    private string _clientModuleID { get; set; }
    private string _apiAuthID { get; set; }
    public string ClientCorporateNo { get; set; }
    public string ClientModuleId
    {
      get => _clientModuleID; set
      {
        _clientModuleID = value;
        // set the api token here
        string fillers = "";
        value = value ?? "WARNING:MIM!!";
        if (value.Length < PortalSecurity.MSACCOAPI.MaxCredentialLength)
        {
          int gap = PortalSecurity.MSACCOAPI.MaxCredentialLength - value.Length;
          for (int i = 0; i < gap; i++)
          {
            fillers += PortalSecurity.MSACCOAPI.FILLER_CHAR;
          }
          APIToken = fillers + string.Join("", value.Substring(0).Reverse());
        }
        else
        {
          APIToken = string.Join("", value.Substring(0, PortalSecurity.MSACCOAPI.MaxCredentialLength).Reverse());
        }
      }
    }
    public List<UserRoles> Roles { get; set; }
    public string APIAuthID
    {
      get => _apiAuthID; set
      {
        string fillers = "";
        value = value ?? "WARNING:UIM!!";
        if(value.Length < PortalSecurity.MSACCOAPI.MaxCredentialLength)
        {
          int gap = PortalSecurity.MSACCOAPI.MaxCredentialLength - value.Length;
          for (int i=0; i<gap; i++)
          {
            fillers += PortalSecurity.MSACCOAPI.FILLER_CHAR;
          }
          _apiAuthID = fillers + string.Join("", value.Substring(0).Reverse());
        }
        else
        {
          _apiAuthID = string.Join("", value.Substring(0, PortalSecurity.MSACCOAPI.MaxCredentialLength).Reverse());
        }
      }
    }
    public string APIToken { get; private set; }

    public class UserRoles : IUserRole
    {
      public string ClientModuleId { get; set; }
      public string AspRoleName { get; set; }
      public string AspRoleId { get; set; }
      public bool IsEnabled { get; set; }
    }

    /// <summary>
    /// The Named Session Variable for this class
    /// Can be called from anywhere in the code.
    /// </summary>
    public static string SessionVaribleName()
    {
      return "ActiveUserParams";
    }
  }
}
