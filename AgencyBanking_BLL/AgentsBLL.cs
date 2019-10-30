using System.Collections.Generic;
using AgencyBanking_DAL;

namespace AgencyBanking_BLL
{
public class AgentsBLL
{
   
        public IEnumerable<AgentModel> GetAgentsByOrganization(string orgname=null)
        {
          var
              //TODO: use where condition
              query =
                 "SELECT TOP (100) [Entry No],[Agent Code] as \"AgentCode\",[Date Registered] as \"DateRegistered\",[Location] as \"Location\",[Active],[Name],[Telephone],[DeviceID] FROM[AGENCY BANKING].[dbo].[Agents]";
            return DapperOrm.QueryGetList<AgentModel>(query);
        }
    }
}
