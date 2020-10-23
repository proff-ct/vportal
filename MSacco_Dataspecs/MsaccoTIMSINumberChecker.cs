using MSacco_Dataspecs.Models;
using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_Dataspecs.Models;

namespace MSacco_Dataspecs.Feature.MsaccoTIMSINumberChecker
{
  namespace Models
  {
    public interface IMSACCO_TIMSI_NumberChecker
    {
      int Id { get; set; }

      string TimsiId { get; set; }

      string PhoneNumber { get; set; }

      DateTime LastUpdated { get; set; }

      DateTime DateLinked { get; set; }

      string Comments { get; set; }
    }

    public interface IMSACCO_TIMSI_NumberCheckerViewModel
    {
      int Id { get; set; }
      string TimsiId { get; set; }
      string PhoneNumber { get; set; }
      string Comments { get; set; }
      DateTime LastUpdated { get; set; }
      DateTime DateLinked { get; set; }
    }
    public interface IMSACCO_TIMSI_RESET_DB_LOG
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
    public interface IMSACCO_TIMSI_RESET_ViewModel
    {
      int LogNo { get; set; }

      string CorporateNo { get; set; }

      string CustomerPhoneNo { get; set; }

      string ActionUser { get; set; }

      string ResetNarration { get; set; }

      DateTime ResetStatusDate { get; set; }
    }
    public enum TIMSI_RESET_STATUS
    {
      Pending,
      Success,
      Failed
    }
  }
  namespace Functions
  {
    public interface IBL_MSACCO_TIMSI_NumberChecker
    {
      IEnumerable<Models.IMSACCO_TIMSI_NumberChecker> GetTIMSIBlockedMembersForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);

      Models.IMSACCO_TIMSI_NumberChecker GetTIMSIMemberRecordForClient(string corporateNo, string memberPhoneNo);
      bool ResetTIMSIMemberRecordForClient(string corporateNo, IMSACCO_TIMSI_RESET_LOG_PARAMS resetParams, out string resetMessage);
    }

    public interface IBL_TIMSI_RESET_LOGGER
    {
      bool LogNewRecordToReset(
        Models.IMSACCO_TIMSI_NumberChecker timsiRecord, ISACCO sacco, IMSACCO_TIMSI_RESET_LOG_PARAMS resetParams, out string logNo);
      bool LogResetOperationStatus(
        ISACCO sacco, string logNo, Models.TIMSI_RESET_STATUS resetStatus, string remarks, out string logResult);
      IEnumerable<IMSACCO_TIMSI_RESET_LOG> GetCustomerLogEntries(string memberPhoneNo, Models.TIMSI_RESET_STATUS? resetStatus = null);
    }
    public interface IBL_TIMSI_RESET_LOG
    {
      IEnumerable<Models.IMSACCO_TIMSI_RESET_DB_LOG> GetResetRecordsForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);

      IEnumerable<Models.IMSACCO_TIMSI_RESET_DB_LOG> GetMemberTIMSIResetRecordsForClient(string corporateNo, string memberPhoneNo);
    }
  }
}
