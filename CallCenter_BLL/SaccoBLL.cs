using CallCenter_DAL;
using CallCenter_Dataspecs;
using CallCenter_Dataspecs.Functions;
using CallCenter_Dataspecs.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace CallCenter_BLL
{
    public class SaccoBLL : IBL_SACCO
    {
        private string query;
        private readonly string tblSacco = Sacco.DBTableName;
        private readonly string[] _saccoColumnNames = new string[]
    {
      "[Corporate No]" , "[Corporate No 2]", "[Sacco Name 1]", "[Mpesa Float], Active"
    };
        public IEnumerable<ISACCO> GetSaccoList()
        {
            query = $@"SELECT {string.Join(",", _saccoColumnNames)}
              FROM {tblSacco}
              WHERE Active='1'";
            return new DapperORM().QueryGetList<Sacco>(query);
        }

        public ISACCO GetSaccoById(int id)
        {
            query = $@"SELECT {string.Join(",", _saccoColumnNames)} FROM {tblSacco} WHERE id='{id.ToString()}'";
            return new DapperORM().QueryGetSingle<Sacco>(query);
        }
        public ISACCO GetSaccoByUniqueParam(string corporateNo = null, string saccoName = null)
        {
            if (!string.IsNullOrEmpty(corporateNo))
            {
                query = $@"SELECT {string.Join(",", _saccoColumnNames)} FROM {tblSacco} WHERE [Corporate No]='{corporateNo}'";
            }
            else if (!string.IsNullOrEmpty(saccoName))
            {
                query = $@"SELECT {string.Join(",", _saccoColumnNames)} FROM {tblSacco} WHERE [Sacco Name 1]='{saccoName}'";
            }
            else
            {
                return null;
            }

            return new DapperORM().QueryGetSingle<Sacco>(query);
        }

        public string GetMSACCOModuleID(string corporateNo)
        {
            string connString = @ConfigurationManager.ConnectionStrings[CC_DBConnectionStrings.VisibilityPortalConnectionStringName].ConnectionString;
            string tblPortalModuleForClient = "PortalModuleForClient";

            DynamicParameters qryParams = new DynamicParameters();
            qryParams.Add("CorporateNo", corporateNo);
            qryParams.Add("PortalModule", "MSacco");
            
            string query = $@"SELECT ClientModuleId 
                            FROM {tblPortalModuleForClient}
                            WHERE ClientCorporateNo=@CorporateNo AND PortalModuleName=@PortalModule";
            
            return new DapperORM(connString).QueryGetSingle<string>(query, qryParams);
        }
    }
}
