using CallCenter_BLL.TransactionPortalServices.Models;
using CallCenter_Dataspecs;
using CallCenter_Dataspecs.Functions;
using CallCenter_Dataspecs.Models;
using CallCenter_Dataspecs.MSSQLOperators;
using CallCenter_Dataspecs.SMSMarketing.Functions;
using CallCenter_Dataspecs.SMSMarketing.Models;
using Dapper;
using Microsoft.Owin;
using MSACCO_SharedFunctions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace CallCenter_BLL
{
    public class SMSMarketingBLL : IBL_SMSMarketing
    {
        private readonly IBL_SACCO _saccoBLL;
        private readonly IOwinContext _owinContext;
        private readonly string _tblBulkSMSFile = "FilePaths";
        private readonly string _tblTrxPortalUsers = "AspNetUsers";
        private readonly string _trxPortalConnString = @ConfigurationManager.ConnectionStrings[CC_DBConnectionStrings.TransactionPortalDBConnectionStringName].ConnectionString;

        public event EventHandler<IFile_QueuedMarketingSMS> OnSMSQueued;
        public event EventHandler<IMSACCO_SMSUnitsPreload> OnSMSUnitsPreloaded;

        public SMSMarketingBLL(IBL_SACCO saccoBLL, IOwinContext owinContext)
        {
            _saccoBLL = saccoBLL ?? throw new ArgumentNullException("NULL SACCO BLL given to SMSMarketingBLL!");
            _owinContext = owinContext;
        }
        private void PreLoadSMSUnits(ISACCO msaccoClient, int numUnits, string trxDescription, string actionUser)
        {
            // 1. load units in db
            string tblBulkSMSTopUp = "[Sacco Bulksms Topup]";
            string MSACCOFinanceUser = "VOLENDIEMA@CORETEC.CO.KE";

            DynamicParameters qryParams = new DynamicParameters();
            qryParams.Add("CorporateNo", msaccoClient.corporateNo);
            qryParams.Add("SMSUnits", numUnits);
            qryParams.Add("TrxDescription", trxDescription);
            qryParams.Add("TopupDate", DateTime.Now);
            qryParams.Add("MSACCOFinanceUser", MSACCOFinanceUser);

            string query = $@"INSERT INTO {tblBulkSMSTopUp}
                (Sacco
                ,SmsCount
                ,[DateTime]
                ,Comments
                ,UserID)
                VALUES
                (@CorporateNo
                ,@SMSUnits
                ,@TopupDate
                ,@TrxDescription
                ,@MSACCOFinanceUser)
                ";

            new DapperORM().ExecuteQuery(query, qryParams);

            // 2. Announce that units loaded
            OnSMSUnitsPreloaded?.Invoke(this, new MSACCOSMSPreload(msaccoClient)
            {
                NumSMSUnits = numUnits,
                TrxDescription = trxDescription,
                ActionUser = actionUser,
                DateLoaded = DateTime.Now
            });
        }

        private string GetSystemAdminForClient(ISACCO msaccoClient)
        {
            string clientMSACCOModuleID = _saccoBLL.GetMSACCOModuleID(msaccoClient.corporateNo);
            if (string.IsNullOrEmpty(clientMSACCOModuleID))
            {
                throw new ApplicationException($"Failed fetching the MSACCO Module ID for {msaccoClient.saccoName_1}");
            }

            string vpDBConn = @ConfigurationManager.ConnectionStrings[CC_DBConnectionStrings.VisibilityPortalConnectionStringName].ConnectionString;
            string tblPortalUserRole = "PortalUserRole";
            string query;

            DynamicParameters qryParams = new DynamicParameters();
            qryParams.Add("ClientModuleID", clientMSACCOModuleID);
            qryParams.Add("PortalRole", "SystemAdmin");
            qryParams.Add("IsRoleEnabled", true);

            // 1. Get first user with enabled SystemAdmin role
            query = $@"SELECT TOP 1 UserId
                FROM {tblPortalUserRole}
                WHERE [ClientModuleId] = @ClientModuleID AND [AspRoleName] = @PortalRole AND IsEnabled = @IsRoleEnabled
                ";
            string userId = new DapperORM(vpDBConn).QueryGetSingle<string>(query, qryParams);


            // 2. Get email address assoicated with that user id
            string tblPortalUsers = "AspNetUsers";
            DynamicParameters qryUserParams = new DynamicParameters();
            qryUserParams.Add("UserID", userId);

            query = $@"SELECT TOP 1 UserName
                FROM {tblPortalUsers}
                WHERE [Id] = @UserID
                ";

            return new DapperORM(vpDBConn).QueryGetSingle<string>(query, qryUserParams) ?? throw new ApplicationException($"Email address for user id {userId} of {msaccoClient.saccoName_1} not found");
        }
        public bool DispatchSMS(string clientCorporateNo, IFile_PortalSMS bulkSMSFile, string actionUser, out string operationMessage)
        {
            bool isDispatched = false;

            if (bulkSMSFile == null ||
                string.IsNullOrEmpty(bulkSMSFile.MessageBody) ||
                bulkSMSFile.ContactList == null ||
                !bulkSMSFile.ContactList.Any())
            {
                operationMessage = "Invalid SMS Data";
                return isDispatched;
            }
            else if (string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(actionUser))
            {
                operationMessage = "Required data missing!";
                return isDispatched;
            }

            ISACCO sacco = _saccoBLL.GetSaccoByUniqueParam(clientCorporateNo);
            if (sacco == null)
            {
                operationMessage = "Failed retrieving client information";
                return isDispatched;
            }

            // Send email using System Admin user of sacco. If no System Admin, then alert user to create System Admin
            string saccoSystemAdminUser = GetSystemAdminForClient(sacco);
            if (string.IsNullOrEmpty(saccoSystemAdminUser))
            {
                operationMessage = $"Failed retrieving System Admin email for {sacco.saccoName_1}. Please confirm whether System Admin has been created for the sacco.";
                return isDispatched;
            }
            
            // verify that bulk sms float is sufficient
            /*IMSACCO_BulkSMSBalance msaccoSMSBalance = GetMSACCOBulkSMSBalance(clientCorporateNo);
            if (msaccoSMSBalance.AvailableBalance < 0)
            {
                operationMessage = $"{sacco.saccoName_1} has sms arrears of {msaccoSMSBalance.ActualBalance} sms units which need to be settled first.";
                return isDispatched;
            }*/

            // pre-load sms units
            PreLoadSMSUnits(sacco, bulkSMSFile.GetTotalSMSToSend(), $"MSACCO Marketing by {actionUser}", actionUser);

            // re-check available balance
           /* msaccoSMSBalance = GetMSACCOBulkSMSBalance(clientCorporateNo);

            if (bulkSMSFile.GetTotalSMSToSend() > msaccoSMSBalance.AvailableBalance)
            {
                AppLogger.LogEvent("SMSMarketingBLL.DispatchSMS", $"{bulkSMSFile.GetTotalSMSToSend()} units have just been pre-loaded but have already been consumed. Current available balance is {msaccoSMSBalance.AvailableBalance}.", null);

                int maxRecipients = msaccoSMSBalance.AvailableBalance / bulkSMSFile.NUM_SMS_PARTS;
                operationMessage = maxRecipients == 0
                    ? string.Format("Available balance({1}) is insufficient to send your message of {0} total sms units", bulkSMSFile.NUM_SMS_PARTS, msaccoSMSBalance.AvailableBalance)
                    : string.Format("Available balance({1} unit(s)) can support {0} MAX number of recipients because your message has {2} sms units per recipient.", maxRecipients, msaccoSMSBalance.AvailableBalance, bulkSMSFile.NUM_SMS_PARTS);

                return isDispatched;
            }*/

            VisibilityPortal_SMSMV smsData = new VisibilityPortal_SMSMV
            {
                Message = bulkSMSFile.MessageBody,
                SMSFileName = bulkSMSFile.FileName,
                Recipients = bulkSMSFile.ContactList.Select(r => r.PhoneNo).ToList()
            };

            VisibilityPortal_SaccoInfo saccoData = new VisibilityPortal_SaccoInfo
            {
                CorporateNo = sacco.corporateNo,
                SaccoName = sacco.saccoName_1
            };

            isDispatched = new TransactionPortalSMSService(_owinContext).UploadBulkSMS(smsData, saccoSystemAdminUser, saccoData);
            operationMessage = "";

            OnSMSQueued?.Invoke(this, new MarketingSMSFile(bulkSMSFile.ContactList)
            {
                FileName = bulkSMSFile.FileName,
                MessageBody = bulkSMSFile.MessageBody,
                SaccoName = sacco.saccoName_1,
                ActionUser = actionUser,
                DateQueued = DateTime.Now
            });

            return isDispatched;
        }

        public List<ISMSRecipient> GenerateRecipientList(List<string> smsRecipients, out string operationMessage)
        {
            if (smsRecipients == null || !smsRecipients.Any())
            {
                operationMessage = "No recipients specified";
                return null;
            }

            Func<string, bool> _checkDigits = smsContact => smsContact.Substring(1).All(c => char.IsDigit(c));

            int numPossibleContacts = 0;
            foreach (string possibleContact in smsRecipients)
            {
                if (_checkDigits(possibleContact))
                {
                    numPossibleContacts++;
                }
            }
            if (numPossibleContacts == 0)
            {
                operationMessage = "No valid contacts";
                return null;
            }


            List<ISMSRecipient> recipients = new List<ISMSRecipient>();
            int numValidContacts = 0;

            smsRecipients.Distinct().ToList().ForEach(phoneNo =>
                {
                    if (phoneNo.Length > RECIPIENT_PHONE_NO_CONFIG.MAX_LENGTH || phoneNo.Length < RECIPIENT_PHONE_NO_CONFIG.MIN_LENGTH)
                    {
                        return;
                    }
                    numValidContacts++;
                    recipients.Add(new SMSRecipient(phoneNo));
                });

            operationMessage = $"Processed {numValidContacts} / {numPossibleContacts} sms recipients.";

            return recipients;
        }

        public IMSACCO_BulkSMSBalance GetMSACCOBulkSMSBalance(string clientCorporateNo)
        {
            return new MSACCO_BulkSMSBalance(AccountSync.BulkSMSCount(clientCorporateNo));
        }

        public IEnumerable<IFile_DispatchedPortalSMS> GetUploadedSMSRecordsForClient(string userEmail, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null)
        {
            lastPage = 0;
            DynamicParameters qryParams = new DynamicParameters();
            //qryParams.Add("CorporateNo", corporateNo);
            qryParams.Add("TrxTypeID", TRANSACTION_FILE_TYPE.SMS);
            qryParams.Add("LoggedInUserEmail", userEmail);

            string query;

            if (paginate)
            {
                query = $@"SELECT f.[Id] as FileNo, [FileName] as SMSFileName, Status, cu.Email as ActionUser, [DateTime] as DateDispatched
                FROM {_tblBulkSMSFile} f
                INNER JOIN {_tblTrxPortalUsers} cu ON cu.Id = [UploadedBy_Id]
                WHERE [TransactionType_Id]=@TrxTypeID AND cu.Email = @LoggedInUserEmail
                ORDER BY FileNo DESC
                OFFSET @PageSize * (@PageNumber - 1) ROWS
                FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

                Select count(f.[Id]) as TotalRecords  
                FROM {_tblBulkSMSFile} f
                INNER JOIN {_tblTrxPortalUsers} cu ON cu.Id = [UploadedBy_Id]
                WHERE [TransactionType_Id]=@TrxTypeID AND cu.Email = @LoggedInUserEmail
                ";

                qryParams.Add("PageSize", pagingParams.PageSize);
                qryParams.Add("PageNumber", pagingParams.PageToLoad);

                using (SqlConnection sqlCon = new SqlConnection(new DapperORM(_trxPortalConnString).ConnectionString))
                {
                    sqlCon.Open();
                    using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
                    {
                        IEnumerable<IFile_DispatchedPortalSMS> records = results.Read<DispatchedSMSFile>();
                        int totalLoanRecords = results.Read<int>().First();

                        lastPage = (int)Math.Ceiling(
                          totalLoanRecords / (decimal)pagingParams.PageSize);
                        return records;
                    }
                }
            }
            else
            {
                query = $@"SELECT f.[Id] as FileNo, [FileName] as SMSFileName, Status, cu.Email as ActionUser, [DateTime] as DateDispatched 
                        FROM {_tblBulkSMSFile} f
                        INNER JOIN {_tblTrxPortalUsers} cu ON cu.Id = [UploadedBy_Id]
                        WHERE [TransactionType_Id]=@TrxTypeID AND ActionUser = @LoggedInUserEmail
                        ORDER BY FileNo DESC";
                return new DapperORM(_trxPortalConnString).QueryGetList<DispatchedSMSFile>(query, qryParams);
            }
        }
    }
}
