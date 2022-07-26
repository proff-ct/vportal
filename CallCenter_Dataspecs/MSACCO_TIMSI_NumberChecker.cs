﻿using CallCenter_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_Dataspecs.Models;

namespace CallCenter_Dataspecs
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
        Models.IMSACCO_TIMSI_NumberChecker timsiRecord, Models.ISACCO sacco, IMSACCO_TIMSI_RESET_LOG_PARAMS resetParams);
      bool LogResetOperationStatus(
        Models.ISACCO sacco, string timsiRecordID, Models.TIMSI_RESET_STATUS resetStatus, string remarks, out string logResult);
      IEnumerable<IMSACCO_TIMSI_RESET_LOG> GetCustomerLogEntries(string memberPhoneNo, Models.TIMSI_RESET_STATUS? resetStatus = null);
    }
  }
}
