using MSacco_Dataspecs.Feature.MsaccoTIMSINumberChecker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_DAL
{
  public class MsaccoTIMSIResetLog : IDBModel, IMSACCO_TIMSI_RESET_DB_LOG
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "MSACCO_TIMSI_RESET_LOG";
    public string databaseName => DBName;
    public string tableName => DBTableName;

    public int LogNo { get; set; }

    public string CorporateNo { get; set; }

    public string CustomerPhoneNo { get; set; }

    public int TIMSINumberCheckerID { get; set; }

    public string SaccoInfo { get; set; }

    public string TIMSIRecord { get; set; }

    public string ActionUser { get; set; }

    public string ResetNarration { get; set; }

    public string ResetStatus { get; set; }

    public DateTime ResetStatusDate { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateLastModified { get; set; }

    public string OperationRemarks { get; set; }
  }
}
