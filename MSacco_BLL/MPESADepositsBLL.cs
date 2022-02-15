using Dapper;
using MSacco_Dataspecs;
using MSacco_Dataspecs.Feature.Transactions.Functions;
using MSacco_Dataspecs.Feature.Transactions.Models;
using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace MSacco_BLL
{
    public class MPESADepositsBLL : IBL_MPESADeposit
    {
        private readonly string _tblUploadedMPESADeposits = "[Mobile Transactions]";
        //private readonly string _trxPortalConnString = @ConfigurationManager.ConnectionStrings[MS_DBConnectionStrings.TransactionPortalDBConnectionStringName].ConnectionString;

        public IEnumerable<IMSACCO_Deposit> GetUploadedDepositRecordsForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null)
        {
            throw new NotImplementedException();
        }

        public bool SubmitTransaction(IFile_MPESA_C2B_Statement c2bStatement, string actionUser, out string operationMessage)
        {
            // 1. Check thet there are records to upload
            // 2. Insert each transaction individually.
            // 2.1 If a transaction fails, skip it and continue to next. Log failure
            // Return true at end of processing
            // Set operation message to indicate partial success if not all records uploaded successfully
            if (string.IsNullOrEmpty(actionUser))
            {
                operationMessage = "Invalid user data";

                AppLogger.LogOperationException("MPESADepositsBLL.SubmitTransaction", "Logged in user details not supplied", new { c2bStatement }, new ArgumentException("Missing logged in user info"));
                return false;
            }
            else if (c2bStatement == null)
            {
                operationMessage = "No data submitted";

                AppLogger.LogOperationException("MPESADepositsBLL.SubmitTransaction", "c2bStatement is null!", null, new ArgumentException("Missing c2bStatement"));
                return false;
            }
            else if (c2bStatement.Deposits == null || !c2bStatement.Deposits.Any())
            {
                operationMessage = "No deposit transactions found";

                AppLogger.LogOperationException("MPESADepositsBLL.SubmitTransaction", "c2bStatement has no transactions!", null, new ArgumentException("Missing c2bStatement.Deposits"));
                return false;
            }

            int totalRecords = c2bStatement.Deposits.Count, countFailedUploads = 0;

            c2bStatement.Deposits.ForEach(trx =>
            {
                try
                {
                    UploadTransaction(trx, c2bStatement.ShortCode, actionUser);
                }
                catch (Exception ex)
                {
                    countFailedUploads++;

                    AppLogger.LogOperationException("MPESADepositsBLL.SubmitTransaction", $"Exception while uploading transaction: {ex.Message}", new { msacco_deposit = trx, client = c2bStatement.Organization, uploadedBy = actionUser }, ex);
                }
            });

            int numUploadedRecords = totalRecords - countFailedUploads;
            if(numUploadedRecords < 1)
            {
                operationMessage = "Something prevented the upload. Kindly contact support immediately!";
                return false;
            }

            operationMessage = $"Uploaded {numUploadedRecords} of {totalRecords} records";
            return true;

        }

        private void UploadTransaction(IMPESADeposit mpesaDeposit, string C2BPaybill, string actionUser)
        {
            string query;

            DynamicParameters qryParams = new DynamicParameters();
            qryParams.Add("CorporateNo", C2BPaybill);
            qryParams.Add("ReceiptNo", mpesaDeposit.ReceiptNo);
            qryParams.Add("AccountNo", mpesaDeposit.AccNo);
            qryParams.Add("MSISDN", mpesaDeposit.PhoneNo);
            qryParams.Add("Trans_Amount", mpesaDeposit.PaidIn);
            qryParams.Add("Description", mpesaDeposit.Details);
            qryParams.Add("Status", mpesaDeposit.TransactionStatus);
            qryParams.Add("Org_Account_Balance", mpesaDeposit.Balance);
            qryParams.Add("Trans_Time", mpesaDeposit.CompletionTime.ToString("dd-MM-yyyy hh:mm:ss tt"));

            query = $@"INSERT INTO {_tblUploadedMPESADeposits}
           ([Corporate No]
           ,[Receipt No]
           ,[Account No]
           ,[MSISDN]
           ,[Trans Amount]
           ,[Description]
           ,[Status]
           ,[Org Account Balance]
           ,[Trans Time]
           ,[Sent to Journal]
           ,[User Transaction Type]
           ,[Date Received]
           ,[Invoice Number]
           ,[Telephone No Affixed]
           ,[Account No Affixed]
           ,[Source])
         SELECT * FROM (
            VALUES(@CorporateNo
               ,@ReceiptNo
               ,@AccountNo
               ,@MSISDN
               ,@Trans_Amount
               ,@Description
               ,@Status
               ,@Org_Account_Balance
               ,@Trans_Time
               ,'0'
               ,'Deposit'
               ,'{DateTime.Now}'
               ,'Deposit'
               ,'No'
               ,'No'
               ,'VisPortal')
        ) AS s([Corporate No]
               ,[Receipt No]
               ,[Account No]
               ,[MSISDN]
               ,[Trans Amount]
               ,[Description]
               ,[Status]
               ,[Org Account Balance]
               ,[Trans Time]
               ,[Sent to Journal]
               ,[User Transaction Type]
               ,[Date Received]
               ,[Invoice Number]
               ,[Telephone No Affixed]
               ,[Account No Affixed]
               ,[Source]) 
        WHERE NOT EXISTS ( 
            SELECT * FROM {_tblUploadedMPESADeposits} t WITH (UPDLOCK) 
            WHERE s.[Receipt No] = t.[Receipt No] 
        )";

            new DapperORM().ExecuteQuery(query, qryParams);

        }
    }
}
