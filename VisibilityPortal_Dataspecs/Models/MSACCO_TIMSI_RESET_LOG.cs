using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisibilityPortal_Dataspecs.Models
{
  public interface IMSACCO_TIMSI_RESET_LOG
  {
     int LogNo { get; set; }

     string CorporateNo { get; set; }

     string CustomerPhoneNo { get; set; }
    int TIMSINumberCheckerID { get; set; }
    string SaccoInfo { get; set; }

     string TIMSIRecord { get; set; }

     string ActionUser { get; set; }

     string ResetNarration { get; set; }

     string ResetStatus { get; set; }

     DateTime ResetStatusDate { get; set; }

     DateTime DateCreated { get; set; }

     DateTime? DateLastModified { get; set; }

     string OperationRemarks { get; set; }

  }

  public interface IMSACCO_TIMSI_RESET_LOG_PARAMS
  {
    string CustomerPhoneNo { get; set; }
    string ResetNarration { get; set; }
    string ActionUser { get; set; }
  }

}
