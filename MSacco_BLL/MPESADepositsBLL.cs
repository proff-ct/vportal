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

        private readonly Func<IMPESADeposit, bool> isDepositTrx = (statementLine) => statementLine.PaidIn > 0;

        public IFile_MPESA_C2B_Statement StatementInProcessing { get; private set; }

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
            // 1. Check if we have a statement in processing
            // 2. If yes, check if the statement in processing is equal to the one passed in
            // 2.1 IS_EQUAL:
            // set the valid deposits equal to that in processing
            // 2.2 ISNOT_EQUAL:
            // set the statement in processing to null
            // goto statement upload flow
            // if NO:
            // set the statement in processing to null
            // goto statement upload flow
            // ELSE (we do not have a statement in processing)
            // goto statement upload flow
            int totalRecords = 0, totalDepositTrx = 0, countFailedUploads = 0;
            List<IMPESADeposit> validDeposits = null;

            if (StatementInProcessing != null && StatementInProcessing.Deposits.Any())
            {
                if (StatementInProcessing.StatementFileName.ToLower() != c2bStatement.StatementFileName.ToLower())
                {
                    StatementInProcessing = null;
                    goto Upload_MSACCO_Deposits;
                }

                validDeposits = StatementInProcessing.Deposits.Where(isDepositTrx).ToList();
                goto Upload_MSACCO_Deposits;

            }
        
        Upload_MSACCO_Deposits:
            if (validDeposits == null)
            {
                if (c2bStatement.Deposits == null || !c2bStatement.Deposits.Any())
                {
                    operationMessage = "No deposit transactions found";

                    AppLogger.LogOperationException("MPESADepositsBLL.SubmitTransaction", "c2bStatement has no transactions!", null, new ArgumentException("Missing c2bStatement.Deposits"));
                    return false;
                }

                totalRecords = c2bStatement.Deposits.Count;
                validDeposits = c2bStatement.Deposits.Where(isDepositTrx).ToList();
                StatementInProcessing = new MPESA_C2B_StatementFile(c2bStatement.ShortCode, validDeposits, c2bStatement.UploadedBy)
                {
                    Organization = c2bStatement.Organization,
                    Operator = c2bStatement.Operator,
                    StatementDate = c2bStatement.StatementDate,
                    StatementFileName = c2bStatement.StatementFileName
                };
            }

            totalDepositTrx = validDeposits.Count;

            for (int i = totalDepositTrx - 1; i >= 0; i--)
            {
                try
                {
                    UploadTransaction(validDeposits.ElementAt(i), c2bStatement.ShortCode, actionUser);
                }
                catch (Exception ex)
                {
                    countFailedUploads++;

                    AppLogger.LogOperationException("MPESADepositsBLL.SubmitTransaction", $"Exception while uploading transaction: {ex.Message}", new { msacco_deposit = validDeposits.ElementAt(i), client = c2bStatement.Organization, uploadedBy = actionUser }, ex);
                }

                int n = StatementInProcessing.Deposits.FindAll(deposits => deposits.ReceiptNo == validDeposits.ElementAt(i).ReceiptNo).Count;

                IMPESADeposit processed = StatementInProcessing.Deposits.Find(deposits => deposits.ReceiptNo == validDeposits.ElementAt(i).ReceiptNo);
                StatementInProcessing.ExpungeTransaction(processed);

                if (n > 1)
                {
                    AppLogger.LogEvent("MPESADepositsBLL.SubmitTransaction", $"Found {n} duplicate trx in StatementInProcessing for receipt number {processed.ReceiptNo}", new { uploadedTrx = validDeposits.ElementAt(i), duplicates = StatementInProcessing.Deposits.FindAll(deposits => deposits.ReceiptNo == validDeposits.ElementAt(i).ReceiptNo) });
                }
                //AppLogger.LogDevNotes("MPESADepositsBLL.SubmitTransaction", "Removed trx from StatementInProcessing", new { trx = validDeposits.ElementAt(i), StatementInProcessing });
            }

            if (!StatementInProcessing.Deposits.Any())
            {
                StatementInProcessing = null;
                AppLogger.LogDevNotes("MPESADepositsBLL.SubmitTransaction", "Set StatementInProcessing to null", null);
            }

            int numUploadedRecords = totalDepositTrx - countFailedUploads;
            if (numUploadedRecords < 1)
            {
                operationMessage = "Something prevented the upload. Kindly contact support immediately!";
                return false;
            }

            operationMessage = $"Total records processed: {totalRecords}{Environment.NewLine}Uploaded {numUploadedRecords} of {totalDepositTrx} paid in records";
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
            qryParams.Add("Trans_Time", mpesaDeposit.CompletionTime.ToString("yyyyMMddhhmmss"));

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
