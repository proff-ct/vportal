using System;
using System.Collections.Generic;
using MSacco_Dataspecs.MSSQLOperators;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_Dataspecs.Feature.Transactions
{
    namespace Models
    {
        public interface IFile_MPESA_C2B_Statement
        {
            string Operator { get; set; }
            string Organization { get; set; }
            DateTime StatementDate { get; set; }
            string StatementFileName { get; set; }
            string UploadedBy { get; }
            string ShortCode { get; }
            List<IMPESADeposit> Deposits { get; }
            void ExpungeTransaction(IMPESADeposit recordToExpunge);
        }
        public interface IMPESADeposit
        {
            string ReceiptNo { get; set; }
            DateTime CompletionTime { get; set; }
            DateTime InitiationTime { get; set; }
            string Details { get; set; }
            string TransactionStatus { get; set; }
            decimal PaidIn { get; set; }
            decimal Withdrawn { get; set; }
            decimal Balance { get; set; }
            bool BalanceConfirmed { get; set; }
            string ReasonType { get; set; }
            string OtherPartyInfo { get; set; }
            string LinkedTransactionID { get; set; }
            string AccNo { get; set; }
            string PhoneNo { get; }

        }

        public class MPESA_C2B_StatementFile : IFile_MPESA_C2B_Statement
        {
            private string _c2bShortCode;
            private List<IMPESADeposit> _transactions;
            public string Operator { get; set; }
            public string Organization { get; set; }
            public DateTime StatementDate { get; set; }
            public string ShortCode { get => _c2bShortCode; private set => _c2bShortCode = value.Trim(); }
            public List<IMPESADeposit> Deposits
            {
                get => _transactions;
                private set => _transactions = value ?? new List<IMPESADeposit>();
            }
            public string StatementFileName { get; set; }

            public string UploadedBy { get; private set; }

            public MPESA_C2B_StatementFile(string c2bPaybill, List<IMPESADeposit> transactions, string actionUser)
            {
                ShortCode = c2bPaybill;
                Deposits = transactions.OrderBy(trx=>trx.CompletionTime).ToList();
                UploadedBy = actionUser;
            }

            public void ExpungeTransaction(IMPESADeposit recordToExpunge)
            {
                if(_transactions != null && _transactions.Any())
                {
                    _transactions.RemoveAll(trx => trx.ReceiptNo == recordToExpunge.ReceiptNo);
                }
            }
        }

        public class StatementFileViewModel
        {
            public string StatementFileName { get; set; }
            public string ShortCode { get; set; }
            public string Operator { get; set; }
            public string Organization { get; set; }
            public DateTime ReportDate { get; set; }
        }

        public class C2BStatementLines : IMPESADeposit
        {
            string _otherPartyInfo;
            public string ReceiptNo { get; set; }
            public DateTime CompletionTime { get; set; }
            public DateTime InitiationTime { get; set; }
            public string Details { get; set; }
            public string TransactionStatus { get; set; }
            public decimal PaidIn { get; set; }
            public decimal Withdrawn { get; set; }
            public decimal Balance { get; set; }
            public bool BalanceConfirmed { get; set; }
            public string ReasonType { get; set; }
            public string OtherPartyInfo {
                get => _otherPartyInfo;
                set
                {
                    _otherPartyInfo = value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        PhoneNo = Functions.MSACCODeposits_BL.ExtractPhoneNumberFromMPESATrx(value);
                    }
                }
            }
            public string LinkedTransactionID { get; set; }
            public string AccNo { get; set; }

            public string PhoneNo { get; private set; }
        }

        public interface IMSACCO_Deposit
        {
            string ReceiptNo { get; set; }
            string AccountNo { get; set; }
            DateTime TransactionDate { get; set; }
            decimal Amount { get; set; }
            string Status { get; set; }
            string OtherPartyInfo { get; set; }
            DateTime DateUploaded { get; set; }
            string StatementFileName { get; set; }
            string Uploadedby { get; set; }
        }
    }
    namespace Functions
    {
        public interface IBL_MPESADeposit
        {
            Models.IFile_MPESA_C2B_Statement StatementInProcessing { get; }
            bool SubmitTransaction(
                Models.IFile_MPESA_C2B_Statement c2bStatement, string actionUser, out string operationMessage);

            IEnumerable<Models.IMSACCO_Deposit> GetUploadedDepositRecordsForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);
        }

        public static class MSACCODeposits_BL
        {
            public const string C2B_MPESA_USER_INFO_DELIMITER = "-";
            /// <summary>
            /// encountered this when I edited the excel file in libreofficecalc
            /// </summary>
            public const string C2B_MPESA_USER_INFO_DELIMITER_2 = "–";
            /// <summary>
            /// this is an 'empty' space delimiter
            /// </summary>
            public const string C2B_MPESA_USER_INFO_DELIMITER_3 = " ";
            public static string ExtractPhoneNumberFromMPESATrx(string otherPartyInfo)
            {
                int IDX_PHONE_NO = 0;
                int IDX_FULL_NAME = 1;
                string phoneNo;

                string[] mpesaUserInfo = otherPartyInfo.Split(new string[] { C2B_MPESA_USER_INFO_DELIMITER }, StringSplitOptions.None);

                // check that we do have a number and iterate over the delimiters

                if (!mpesaUserInfo[IDX_PHONE_NO].Trim().All(char.IsDigit))
                {
                    mpesaUserInfo = otherPartyInfo.Split(new string[] { C2B_MPESA_USER_INFO_DELIMITER_2 }, StringSplitOptions.None);
                    if (!mpesaUserInfo[IDX_PHONE_NO].Trim().All(char.IsDigit))
                    {
                        mpesaUserInfo = otherPartyInfo.Split(new string[] { C2B_MPESA_USER_INFO_DELIMITER_3 }, StringSplitOptions.None);
                        if (!mpesaUserInfo[IDX_PHONE_NO].Trim().All(char.IsDigit))
                        {
                            phoneNo = "Number Parse Failure";
                        }
                        else
                        {
                            phoneNo = mpesaUserInfo[IDX_PHONE_NO].Trim();
                        }
                    }
                    else
                    {
                        phoneNo = mpesaUserInfo[IDX_PHONE_NO].Trim();
                    }
                }
                else
                {
                    phoneNo = mpesaUserInfo[IDX_PHONE_NO].Trim();
                }

                return MSACCOToolbox.ParsePhoneNumberToMSACCOFormat(phoneNo);
            }
        }

    }

}
