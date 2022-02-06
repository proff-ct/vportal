using Dapper;
using Microsoft.Owin;
using MSacco_BLL.TransactionPortalServices.Models;
using MSacco_Dataspecs;
using MSacco_Dataspecs.Feature.PortalSMS.Functions;
using MSacco_Dataspecs.Feature.PortalSMS.Models;
using MSacco_Dataspecs.Functions;
using MSacco_Dataspecs.Models;
using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace MSacco_BLL
{
    public class PortalSMSBLL : IBL_PortalSMS
    {
        private readonly IBL_SACCO _saccoBLL;
        private readonly IOwinContext _owinContext;
        private readonly string _tblBulkSMSFile = "FilePaths";
        private readonly string _tblTrxPortalUsers = "AspNetUsers";
        private readonly string _trxPortalConnString = @ConfigurationManager.ConnectionStrings[MS_DBConnectionStrings.TransactionPortalDBConnectionStringName].ConnectionString;
        public PortalSMSBLL(IBL_SACCO saccoBLL, IOwinContext owinContext)
        {
            _saccoBLL = saccoBLL ?? throw new ArgumentNullException("NULL SACCO BLL given to PortalSMSBLL!");
            _owinContext = owinContext;
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

            isDispatched = new TransactionPortalSMSService(_owinContext).UploadBulkSMS(smsData, actionUser, saccoData);
            operationMessage = "";

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
                query = $@"SELECT f.[Id] as FileNo, [FileName] as SMSFileName, [Status], cu.Email as ActionUser, [DateTime] as DateDispatched
                FROM {_tblBulkSMSFile} f
                INNER JOIN {_tblTrxPortalUsers} cu ON cu.Id = [UploadedBy_Id]
                WHERE [TransactionType_Id]=@TrxTypeID AND ActionUser = @LoggedInUserEmail
                ORDER BY FileNo DESC
                OFFSET @PageSize * (@PageNumber - 1) ROWS
                FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

                Select count([Id]) as TotalRecords  
                FROM {_tblBulkSMSFile}
                WHERE [TransactionType_Id]=@TrxTypeID AND [Email] = @LoggedInUserEmail
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
                //query = $@"SELECT * FROM {_tblMsaccoBankTransfer} WHERE [Corporate No] = @CorporateNo ";
                query = $@"SELECT f.[Id] as FileNo, [FileName] as SMSFileName, [Status], cu.Email as ActionUser, [DateTime] as DateDispatched 
                        FROM {_tblBulkSMSFile} f
                        INNER JOIN {_tblTrxPortalUsers} cu ON cu.Id = [UploadedBy_Id]
                        WHERE [TransactionType_Id]=@TrxTypeID AND ActionUser = @LoggedInUserEmail
                        ORDER BY FileNo DESC";
                return new DapperORM(_trxPortalConnString).QueryGetList<DispatchedSMSFile>(query, qryParams);
            }
        }
    }
}
