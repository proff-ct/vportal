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
            string ShortCode { get; }
            List<IMPESADeposit> Deposits { get; }
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

            public MPESA_C2B_StatementFile(string c2bPaybill, List<IMPESADeposit> transactions)
            {
                ShortCode = c2bPaybill;
                Deposits = transactions;
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
            string _phoneNo, _otherPartyInfo;
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
                    if (!string.IsNullOrEmpty(_otherPartyInfo))
                    {
                        _phoneNo = Functions.MSACCODeposits_BL.ExtractPhoneNumberFromMPESATrx(_otherPartyInfo);
                    }
                }
            }
            public string LinkedTransactionID { get; set; }
            public string AccNo { get; set; }

            public string PhoneNo => _phoneNo;
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
            bool SubmitTransaction(
                Models.IFile_MPESA_C2B_Statement c2bStatement, string actionUser, out string operationMessage);

            IEnumerable<Models.IMSACCO_Deposit> GetUploadedDepositRecordsForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);
        }

        public static class MSACCODeposits_BL
        {
            public static string C2B_MPESA_USER_INFO_DELIMITER => "-";
            public static string ExtractPhoneNumberFromMPESATrx(string otherPartyInfo)
            {
                int IDX_PHONE_NO = 0;
                int IDX_FULL_NAME = 1;

                string[] mpesaUserInfo = otherPartyInfo.Split(new string[] { C2B_MPESA_USER_INFO_DELIMITER }, StringSplitOptions.None);

                return MSACCOToolbox.ParsePhoneNumberToMSACCOFormat(mpesaUserInfo[IDX_PHONE_NO].Trim());
            }
        }

    }

}
