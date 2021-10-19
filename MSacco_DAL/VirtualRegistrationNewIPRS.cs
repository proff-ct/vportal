using MSacco_Dataspecs.Feature.IPRS;
using MSacco_Dataspecs.Feature.VirtualRegistration.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_DAL
{
  public class VirtualRegistrationNewIPRS : IVirtualRegistrationIPRS, IDBModel
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "VirtualRegistrationsNewIPRS";
    //public string databaseName => DBName;
    //public string tableName => DBTableName;
    public int Entry_No { get; set; }
    public string Corporate_No { get; set; }
    public string MSISDN { get; set; }
    public string Surname { get; set; }
    public string Othernames { get; set; }
    public string Citizenship { get; set; }
    public string Gender { get; set; }
    public DateTime Date_Of_Birth { get; set; }
    public string IDNum { get; set; }
    public string Serial_Number { get; set; }
    public bool IDisValid { get; set; }
    public bool Sent_To_IPRS { get; set; }
    public bool Complete { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }
    public DateTime UssdDateOfBirth { get; set; }
    public string Mpesa_Names { get; set; }
    public DateTime Registration_Date { get; set; }
  }
}
