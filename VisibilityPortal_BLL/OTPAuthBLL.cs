using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using VisibilityPortal_DAL;
using VisibilityPortal_Dataspecs.OTPAuth.Functions;
using VisibilityPortal_Dataspecs.OTPAuth.Models;

namespace VisibilityPortal_BLL
{
    public class OTPAuthBLL : IBL_OTPAuth
    {
#if DEBUG
        private readonly string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_testing"].ConnectionString;
#else
    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_prod"].ConnectionString;
#endif
        private readonly string _tblPortalOTP = PortalOTP.DBTableName;
        private string _query;
        private DynamicParameters _queryParameters;
        private Func<IOTP, bool> _checkExpired = expiredOTP => (DateTime.Now - expiredOTP.CreatedAt).Minutes >= 5;
        public string GenerateOTP(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User ID not available");
            }

            List<IOTP> pendingOTP = GetPendingOTP(userId);
            if (pendingOTP == null)
            {
                goto gen_otp;
            }
            else
            {
                int numPending = pendingOTP.Count;
                switch (numPending)
                {
                    case 0:
                        goto gen_otp;
                    case 1:
                        IOTP pendingRequest = pendingOTP.First();
                        if (_checkExpired(pendingRequest))
                        {
                            UpdateStatus(pendingRequest, OTP_STATUS.Expired);
                            goto gen_otp;
                        }
                        // return current pending request if not yet expired
                        return pendingRequest.Code;

                    default:
                        // somehow we have multiple pending requests. So invalidate all but the most recent
                        IOTP latestRequest = pendingOTP.OrderByDescending(pending => pending.CreatedAt)
                            .First();

                        pendingOTP.OrderByDescending(pending => pending.CreatedAt)
                            .Skip(1)
                            .ToList()
                            .ForEach(invalid => UpdateStatus(invalid, OTP_STATUS.Expired));

                        return latestRequest.Code;
                }

            }

        gen_otp:
            string otp = GenerateOTP();

            if (SendToStorage(userId, otp))
            {
                return otp;
            }

            throw new ApplicationException("Failed generating auth token");

        }

        public bool ValidateOTP(string userId, string otp, out string validationMsg)
        {
            bool isValidated = false;

            try
            {
                IOTP userOTP = RetrieveFromStorage(userId, otp);
                if (userOTP == null)
                {
                    validationMsg = "Validation failed";
                    return isValidated;
                }
                // check that OTP is not expired
                if (_checkExpired(userOTP))
                {
                    validationMsg = "Invalid OTP";
                    return isValidated;
                }

                // check that OTP match
                if(userOTP.Code != otp)
                {
                    validationMsg = "Fraudulent OTP";
                    return isValidated;
                }

                validationMsg = "";
                isValidated = true;
            }
            catch (Exception ex)
            {
                validationMsg = "OTP Validation Failed";
                AppLogger.LogOperationException("OTPAuthBLL.ValidateOTP", ex.Message, new { userId, otp }, ex);
            }

            return isValidated;
        }

        private bool SendToStorage(string userId, string otp)
        {
            bool isInserted = false;

            _query = $@"INSERT INTO {_tblPortalOTP}
                       ([Code]
                       ,[UserId]
                       ,[CreatedAt]
                       ,[Status])
                    VALUES 
                       (@Code
                       ,@UserId
                       ,@CreatedAt
                       ,@Status)";

            _queryParameters = new DynamicParameters();
            _queryParameters.Add("Code", otp);
            _queryParameters.Add("UserId", userId);
            _queryParameters.Add("CreatedAt", DateTime.Now);
            _queryParameters.Add("Status", nameof(OTP_STATUS.Pending));

            try
            {
                new DapperORM().ExecuteQuery(_query, _queryParameters);
                isInserted = true;
            }
            catch (Exception ex)
            {
                AppLogger.LogOperationException("OTPAuthBLL.SendToStorage", ex.Message, new { userId, otp }, ex);
            }

            return isInserted;
        }

        private string GenerateOTP()
        {
            Random rnd = new Random();
            return rnd.Next(10000, 99999).ToString();
        }

        private List<IOTP> GetPendingOTP(string userId)
        {
            _query = $@"SELECT * FROM {_tblPortalOTP} 
                      WHERE [UserId] = @UserID
                      AND [Status] = @OTP_STATUS";

            _queryParameters = new DynamicParameters();
            _queryParameters.Add("UserID", userId);
            _queryParameters.Add("OTP_STATUS", nameof(OTP_STATUS.Pending));

            return AutoMapper.Mapper.Map<IEnumerable<IDB_OTP>, IEnumerable<IOTP>>(new DapperORM().QueryGetList<PortalOTP>(_query, _queryParameters)).ToList();

        }

        private IOTP RetrieveFromStorage(string userId, string otp)
        {
            _query = $@"SELECT * FROM {_tblPortalOTP} 
                      WHERE [UserId] = @UserID
                      AND [Code] = @OTP";

            _queryParameters = new DynamicParameters();
            _queryParameters.Add("UserID", userId);
            _queryParameters.Add("OTP", otp);

            return AutoMapper.Mapper.Map<IDB_OTP, IOTP>(new DapperORM().QueryGetSingle<PortalOTP>(_query, _queryParameters));
        }
        private bool UpdateStatus(IOTP userOTP, OTP_STATUS statusToSet)
        {
            bool isUpdated = false;

            _query = $@"UPDATE {_tblPortalOTP} 
                      SET [Status] = @OTP_STATUS, [LastUpdatedAt] = @UpdatedAt 
                      WHERE [UserId] = @UserID
                      AND [Code] = @OTP";

            _queryParameters = new DynamicParameters();
            _queryParameters.Add("UserID", userOTP.UserId);
            _queryParameters.Add("OTP", userOTP.Code);
            _queryParameters.Add("OTP_STATUS", statusToSet.ToString());
            _queryParameters.Add("UpdatedAt", DateTime.Now);

            try
            {
                new DapperORM().ExecuteQuery(_query, _queryParameters);
                isUpdated = true;
            }
            catch (Exception ex)
            {
                AppLogger.LogOperationException("OTPAuthBLL.UpdateStatus", ex.Message, new { userOTP, statusToSet }, ex);
            }

            return isUpdated;
        }
        private class UserOTP : IOTP
        {
            public string Code { get; set; }
            public string UserId { get; set; }
            public DateTime CreatedAt { get; set; }
            public OTP_STATUS Status { get; set; }
        }
    }
}
