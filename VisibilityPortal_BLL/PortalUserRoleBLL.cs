using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_BLL.Utilities;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL
{
  public class PortalUserRoleBLL
  {
#if DEBUG
    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_testing"].ConnectionString;
#else
    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_prod"].ConnectionString;
#endif
    
    string _query;
    string _tblPortalUserRole = PortalUserRole.DBTableName;
    public IEnumerable<PortalUserRole> GetListOfPortalUserRoles()
    {
      _query = $@"SELECT * FROM {_tblPortalUserRole}";
      return new DapperORM(_connString).QueryGetList<PortalUserRole>(_query);
    }
    public IEnumerable<PortalUserRole> GetPortalUserRoleListForUser(string userId)
    {
      _query = $@"SELECT * FROM {_tblPortalUserRole} WHERE UserId='{userId}'";
      return new DapperORM(_connString).QueryGetList<PortalUserRole>(_query);
    }

    public bool Save(PortalUserRole portalUserRole, ModelOperation modelOp)
    {
      if (!ValidatePortalUserRole(portalUserRole)) return false;
      switch (modelOp)
      {
        case ModelOperation.AddNew:
          _query = $@"INSERT INTO {_tblPortalUserRole}
                  (ClientModuleId,
                  UserId,
                  AspRoleId,
                  CreatedBy)
                  VALUES(
                  '{portalUserRole.ClientModuleId}',
                  '{portalUserRole.UserId}',
                  '{portalUserRole.AspRoleId}',
                  '{portalUserRole.CreatedBy}')
                ";
          break;

        case ModelOperation.Update:
          _query = $@"UPDATE {_tblPortalUserRole} 
          SET
            ModifiedBy='{portalUserRole.ModifiedBy}'
          WHERE Id='{portalUserRole.Id}'";
          break;
      };
      try
      {
        new DapperORM().ExecuteQuery(_query);
        return true;
      }
      catch (Exception ex)
      {
        // should log the exception
        return false;
      }
    }
    public bool Delete(string portalUserRoleId)
    {
      _query = $@"DELETE FROM {_tblPortalUserRole} WHERE Id='{portalUserRoleId}'";
      try
      {
        new DapperORM().ExecuteQuery(_query);
        return true;
      }
      catch (Exception ex)
      {
        // log the exception
        return false;
      }
    }
    private bool ValidatePortalUserRole(PortalUserRole portalUserRole)
    {
      return Validators.ValidateObject(portalUserRole);
    }

  }
}
