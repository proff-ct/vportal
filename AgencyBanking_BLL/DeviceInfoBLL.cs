using System.Collections.Generic;
using AgencyBanking_BLL.util;
using AgencyBanking_DAL;
using Dapper;
using System.Activities.Statements;
using System.Data.SqlClient;

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

        /// <summary>
        /// Creates a new Device into the Database
        /// </summary>
        /// <param name="model">Device Model</param>
        /// <returns></returns>

        public string InsertNewDevice(DeviceModel model)
        {
            string query =
                "INSERT INTO [dbo].[DeviceInfo]         ([Name]        ,[IMEI]        ,[Enabled]        ,[TypeCode]        ,[TypeName]        ,[Assigned]        ,[Organization]        ,[Region])  VALUES  " +
                "      (@name,@imei,0,@TypeCode,@TypeName,0,@org,@region)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("name", model.Name);
            parameters.Add("imei", model.IMEI);
            parameters.Add("@TypeCode", model.TypeCode);
            parameters.Add("@TypeName", model.TypeName);
            parameters.Add("@org", model.Organization);
            parameters.Add("@region", model.Region);
            if (DapperOrm.ExecuteInsert(query, parameters) > 0)
            {
                return new StatusMessage
                {
                    Code = "200",
                    Message = "Device was added successfully"
                }.toJson();
            }

            return new StatusMessage()
            {
                Code = "500",
                Message = "There was a problem adding the device"
            }.toJson();

        }

        public bool AssignDevice(DeviceAssignModel model)
        {

            using (SqlConnection connection = new SqlConnection(new DBConnection().ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {   
                    DynamicParameters parameters  = new DynamicParameters();
                    parameters.Add("imei",model.DeviceImei);
                    DeviceInfo info = DapperOrm.QueryGetSingle<DeviceInfo>("select id from deviceinfo where imei =@imei",parameters);
                    if (info is null)
                    {
                        return false;
                    }
                    string query = "update agents set deviceid = @id where [agent code] = @agentcode";
                    parameters = new DynamicParameters();
                    parameters.Add("agentcode",model.AgentCode);
                    parameters.Add("id", info.ID);
                    if (connection.Execute(query,parameters,transaction:transaction) > 0)
                    {
                        parameters.Add("imei",model.DeviceImei);
                        query = "update deviceinfo set enabled = 1 where imei = @imei";
                        if (connection.Execute(query, parameters, transaction: transaction) < 0)
                        {
                            transaction.Rollback();
                        }
                        transaction.Commit();
                        return true;

                    }

                    transaction.Rollback();
                }
                connection.Close();
            }

            return false;



        }
    }
}