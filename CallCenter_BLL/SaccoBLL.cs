using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallCenter_DAL;

namespace CallCenter_BLL
{
  public class SaccoBLL
  {
    string query;
    string tblSacco = Sacco.DBTableName;
    string[] _saccoColumnNames = new string[]
    {
      "[Corporate No]" , "[Corporate No 2]", "[Sacco Name 1]", "[Mpesa Float]"
    };
    public IEnumerable<Sacco> GetSaccoList()
    {
      query = $@"SELECT {string.Join(",",_saccoColumnNames)}
             FROM {tblSacco}";
      return DapperORM.QueryGetList<Sacco>(query);
    }
    
    public Sacco GetSaccoById(int id)
    {
      query = $@"SELECT {string.Join(",", _saccoColumnNames)} FROM {tblSacco} WHERE id='{id.ToString()}'";
      return DapperORM.QueryGetSingle<Sacco>(query);
    }
    public Sacco GetSaccoByUniqueParam(string corporateNo = null, string saccoName = null)
    {
      if (!string.IsNullOrEmpty(corporateNo))
      {
        query = $@"SELECT {string.Join(",", _saccoColumnNames)} FROM {tblSacco} WHERE [Corporate No]='{corporateNo}'";
      }
      else if (!string.IsNullOrEmpty(saccoName))
      {
        query = $@"SELECT {string.Join(",", _saccoColumnNames)} FROM {tblSacco} WHERE [Sacco Name 1]='{saccoName}'";
      }
      else return null;
      return DapperORM.QueryGetSingle<Sacco>(query);
    }
    
  }
}
