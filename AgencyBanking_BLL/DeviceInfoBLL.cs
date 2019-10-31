using System.Collections.Generic;
using AgencyBanking_DAL;

namespace AgencyBanking_BLL
{
    class DeviceInfoBLL
    {
        string query;
        private string table = DeviceInfo.Table;
/// <summary>
/// Returns a list of Devices for a given organization Name
/// 
/// </summary>
/// <param name="orgname">Organization Name</param>
/// <returns>a List of Devices</returns>
        public IEnumerable<DeviceInfo> GetDevicesByOrganization(string orgname)
        {
            query = $"select * from {table} where Organization = '{orgname}'";
            return DapperOrm.QueryGetList<DeviceInfo>(query);
        }
    }
}
