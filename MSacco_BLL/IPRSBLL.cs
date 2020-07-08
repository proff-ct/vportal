using MSacco_DAL;
using MSacco_Dataspecs.Feature.IPRS;
using MSacco_Dataspecs.Feature.IPRS.VirtualRegistrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSacco_BLL
{
  public class IPRSBLL : IBL_IPRS
  {
    private string _query;
    private readonly string _tblIPRSRecords = VirtualRegistrationNewIPRS.DBTableName;
    public List<IIPRS_Record> GetIPRSRecords(string corporateNo, int entryNo = 0)
    {
      _query = entryNo > 0
        ? $@"SELECT * FROM {_tblIPRSRecords} WHERE [Entry No]='{entryNo}'"
        : $@"SELECT * FROM {_tblIPRSRecords} WHERE [Corporate No]='{corporateNo}'";

      return new DapperORM().QueryGetList<VirtualRegistrationNewIPRS>(_query).OfType<IIPRS_Record>().ToList();
    }

    public IIPRS_Record PerformIPRSLookup(string corporateNo, string idNumber, string phoneNumber)
    {
      if (phoneNumber.Length < 10) throw new ApplicationException(
        $"Phone number length is invalid. Is {phoneNumber.Length} instead of 10");

      phoneNumber =string.Format("+254{0}", phoneNumber.Substring(phoneNumber.Length - 9));

      string newRegRecStatus = "Pending";
      IIPRS_Record regRecord = null;
      int waitMiliseconds = 3000;


      //throw new NotImplementedException();
      // 1. create vreg record
      // 2. wait for x sec for record to be updated
      // 3. return updated record
      _query = $@"DECLARE @IDs TABLE(ID INT);

        INSERT INTO {_tblIPRSRecords}
        ([Corporate No], MSISDN, Status, IDNum, RegistrationChannel, [Registration Date], UssdDateOfBirth)
        OUTPUT inserted.[Entry No] INTO @IDs(ID)
         VALUES (
          '{corporateNo}',
          '{phoneNumber}',
          '{newRegRecStatus}',
          '{idNumber}',
          '{(int)RegistrationChannels.IPRSOnly}',
          '{ DateTime.Now}',
          '{ DateTime.Now}'
        );
        
        SELECT * FROM {_tblIPRSRecords} WHERE [Entry No]=(SELECT ID FROM @IDs)
        ";

      regRecord = new DapperORM().QueryGetSingle<VirtualRegistrationNewIPRS>(_query);
      Thread.Sleep(waitMiliseconds);

      return GetIPRSRecords(corporateNo, regRecord.Entry_No).FirstOrDefault();
    }
  }
}
