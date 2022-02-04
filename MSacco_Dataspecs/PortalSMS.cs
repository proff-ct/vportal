using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_Dataspecs.Feature.PortalSMS
{
    namespace Models
    {
        public interface IFile_PortalSMS
        {
            string FileName { get; set; }
            string MessageBody { get; set; }
            List<ISMSRecipient> ContactList { get; }
        }
        public interface IFile_DispatchedPortalSMS
        {
            int FileNo { get; set; }
            string SMSFileName { get; set; }
            string Message { get; set; }
            string Status { get; set; }
            DateTime DateDispatched { get; set; }
            string ActionUser { get; set; }
        }
        public interface ISMSRecipient
        {
            string PhoneNo { get; }
        }

        public class PortalSMSFile : IFile_PortalSMS
        {
            private List<ISMSRecipient> _recipients;
            public List<ISMSRecipient> ContactList
            {
                get => _recipients;
                private set => _recipients = value ?? new List<ISMSRecipient>();
            }
            public string FileName { get; set; }
            public string MessageBody { get; set; }

            public PortalSMSFile(List<ISMSRecipient> smsRecipients)
            {
                ContactList = smsRecipients;
            }
        }

        public class SMSRecipient : ISMSRecipient
        {
            public string PhoneNo { get; private set; }
            public SMSRecipient(string phoneNo)
            {
                if (string.IsNullOrEmpty(phoneNo))
                {
                    throw new ArgumentException("PhoneNo not supplied!");
}
                else
                {
                    PhoneNo = phoneNo;
                }
            }
        }
        public class DispatchedSMSFile : IFile_DispatchedPortalSMS
        {
            public int FileNo { get; set; }
            public string SMSFileName { get; set; }
            public string Message { get; set; }
            public string Status { get; set; }
            public DateTime DateDispatched { get; set; }
            public string ActionUser { get; set; }
        }
        public class PortalSMSFileViewModel
        {
            public string FileName { get; set; }
            public string Message { get; set; }
            public List<string> RecipientList { get; set; }
        }

        public static class RECIPIENT_PHONE_NO_CONFIG
        {
            public const int MAX_LENGTH = 15;
            public const int MIN_LENGTH = 9;
        }

        public enum TRANSACTION_FILE_TYPE
        {
            SMS = 2
        }
    }
    namespace Functions
    {
        public interface IBL_PortalSMS
        {
            List<Models.ISMSRecipient> GenerateRecipientList(List<string> smsRecipients, out string operationMessage);

            bool DispatchSMS(string clientCorporateNo, Models.IFile_PortalSMS bulkSMSFile, string actionUser, out string operationMessage);

            IEnumerable<Models.IFile_DispatchedPortalSMS> GetUploadedSMSRecordsForClient(string userEmail, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);
        }


    }

}
