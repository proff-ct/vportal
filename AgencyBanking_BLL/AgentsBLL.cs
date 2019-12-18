using System.Collections.Generic;
using AgencyBanking_DAL;
using Dapper;

namespace AgencyBanking_BLL
{
public class AgentsBLL
{
   
        public IEnumerable<AgentModel> GetAgentsByOrganization(string orgname)
        {
          DynamicParameters par = new DynamicParameters();
          par.Add("number",orgname);
             string query =
                 "SELECT TOP (100) [Entry No],[Agent Code] as \"AgentCode\",[Date Registered] as \"DateRegistered\",[Location] as \"Location\",[Active],[Name],[Telephone],[DeviceID] FROM[AGENCY BANKING].[dbo].[Agents] where [bank no] = @number";
            return DapperOrm.QueryGetList<AgentModel>(query,par);
        }
    }
}
