using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL
{
  public class SaccoBLL
  {
    string _connString = @ConfigurationManager.ConnectionStrings["saccoDB_prod"].ConnectionString;
    string _query;
    string _tblSacco = Sacco.DBTableName;
    string[] _saccoColumnNames = new string[]
    {
      "[Corporate No]" , "[Corporate No 2]", "[Sacco Name 1]"
    };
    public IEnumerable<Sacco> GetSaccoList()
    {
      _query = $@"SELECT {string.Join(",",_saccoColumnNames)}
             FROM {_tblSacco} WHERE Active='1'";
      return new DapperORM(_connString).QueryGetList<Sacco>(_query);
    }
    
    public Sacco GetSaccoById(int id)
    {
      _query = $@"SELECT {string.Join(",", _saccoColumnNames)} FROM {_tblSacco} WHERE id='{id.ToString()}'";
      return new DapperORM(_connString).QueryGetSingle<Sacco>(_query);
    }
    public Sacco GetSaccoByUniqueParam(string corporateNo = null, string saccoName = null)
    {
      if (!string.IsNullOrEmpty(corporateNo))
      {
        _query = $@"SELECT {string.Join(",", _saccoColumnNames)} FROM {_tblSacco} WHERE [Corporate No]='{corporateNo}'";
      }
      else if (!string.IsNullOrEmpty(saccoName))
      {
        _query = $@"SELECT {string.Join(",", _saccoColumnNames)} FROM {_tblSacco} WHERE [Sacco Name 1]='{saccoName}'";
      }
      else return null;
      return new DapperORM(_connString).QueryGetSingle<Sacco>(_query);
    }
    
  }
}
