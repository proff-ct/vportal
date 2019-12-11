using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels
{
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUser : IdentityUser
  {
    /**
     * The annotations on the CreatedOn and ModifiedOn fields are to address
     * an error: "The conversion of a datetime2 data type to a datetime data
     * type resulted in an out-of-range value." that comes about because I
     * have specified a database generated value as the default value for
     * those columns i.e. "=SYSDATETIME()" in the db.
     * 
     * Explanation on why this solution works:
     * The question on stackoverflow: https://stackoverflow.com/q/1331779
     * The answer that provided the rationale: https://stackoverflow.com/a/28880616
     * 
     * For additional understanding of this attribute, I referenced;
     * Question: https://stackoverflow.com/questions/15585330/calculated-column-in-ef-code-first
     * Referenced the answer(https://stackoverflow.com/a/15585492) 
     * by Gert Anorld (https://stackoverflow.com/users/861716/gert-arnold)
     * 
     * Matthew Adote
     * 27_Oct_2019 2000HRS
     */
    private CoretecClientBLL _coretecClientBLL = new CoretecClientBLL();
    private PortalUserRoleBLL _portalUserRoleBLL = new PortalUserRoleBLL();
    public string ClientCorporateNo { get; set; }
    public string CreatedBy { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedOn { get; private set; }
    public string ModifiedBy { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime ModifiedOn { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime DateEmailConfirmed { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public bool IsDefaultPassword { get; set; }
    public virtual IEnumerable<PortalRole> PortalRoles { get; set; }
    //public ApplicationUser()
    //{
    //  PortalRoles = Mapper.Map<IEnumerable<PortalRole>>(
    //    _portalUserRoleBLL.GetPortalUserRoleListForUser(Id));
    //}
    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    {
      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      ClaimsIdentity userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
      // Add custom user claims here
      List<string> appRoles = manager.GetRoles(Id).ToList();
      PortalRoles = Mapper.Map<IEnumerable<PortalRole>>(
        _portalUserRoleBLL.GetPortalUserRoleListForUser(Id));
      PortalRoles.ToList().ForEach(pr =>
      {
        PortalModuleForClient clientModule = _coretecClientBLL.GetPortalModuleForClient(
          pr.ClientModuleId);
        if (appRoles.Contains(pr.AspRoleName))
        {
          userIdentity.AddClaim(new Claim(clientModule.PortalModuleName, pr.AspRoleName));
        }
      });

      return userIdentity;
    }

  }
}
