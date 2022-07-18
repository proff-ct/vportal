using CallCenter_Dataspecs.SMSMarketing.Functions;
using CallCenter_Dataspecs.SMSMarketing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace CallCenter_Dataspecs.MSACCOFinanceAuditor
{
    namespace Models
    {
        public interface IMSACCOFinanceAuditor
        {
            void Subscribe(IBL_SMSMarketing msaccoSMSMarketingBLL);
        }

        #region Implementation

        public class MSACCOFinanceAuditor : IMSACCOFinanceAuditor
        {
            public void Subscribe(IBL_SMSMarketing msaccoSMSMarketingBLL)
            {
                msaccoSMSMarketingBLL.OnSMSQueued += LogMarketingSMS;
                msaccoSMSMarketingBLL.OnSMSUnitsPreloaded += SendEmailAlert;
            }

            private void LogMarketingSMS(object sender, IFile_QueuedMarketingSMS e)
            {
                AppLogger.LogEvent("LogMarketingSMS", $"MSACCO Marketing SMS has been queued for dispatch", e);
            }
            private void SendEmailAlert(object sender, IMSACCO_SMSUnitsPreload e)
            {
                AppLogger.LogEvent("LogMarketingSMS", $"Pre-loaded {e.NumSMSUnits} Bulk SMS Units for {e.Client.saccoName_1}", null);

                // send email here
            }
        }

        #endregion
    }

    namespace Functions { }
}
