using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_DAL
{
  public class MSaccoSalaryAdvance : IDBModel
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "MSaccoSalaryAdvance";
    public string tableName => DBName;
    public string databaseName => DBTableName;
    public int Entry_No { get; set; }
    public string Status { get; set; }
    public string Account_No { get; set; }
    public string Telephone_No { get; set; }
    public decimal? Amount { get; set; }
    public string Comments { get; set; }
    public string SESSION_ID { get; set; }
    public DateTime? Transaction_Date { get; set; }
    public string Sent_To_Journal { get; set; }
    public DateTime? Date_Sent_To_Journal { get; set; }
    public DateTime? Time_Sent_To_Journal { get; set; }
    public string Corporate_No { get; set; }
    public string Member_No { get; set; }
    public string Staff_No { get; set; }
    public string Client_Code { get; set; }

    public decimal? Account_Balance { get; set; }

    public int? Repayment_Period { get; set; }

    public double? Mounthly_Installments { get; set; }

    public double? Processing_fee { get; set; }

    public double? Max_Loan { get; set; }

    public double? Min_Loan { get; set; }

    public string Type { get; set; }

    public double? Net_Pay { get; set; }

    public string Recovery { get; set; }

    public long? Source { get; set; }

    public string Loan_type { get; set; }

    public string Loan_Name { get; set; }

    public double? KG { get; set; }

    public string Client_Name { get; set; }

    public bool? Bonus_based { get; set; }

    public int? No_of_Guarantors { get; set; }

    public double? KG_Guaranteed { get; set; }

    public string Remarks { get; set; }

    public string Loan_Status { get; set; }

    public int? KG_Used { get; set; }

    public bool? G_Appraised { get; set; }

    public bool? Disbursed { get; set; }

    
  }
}
