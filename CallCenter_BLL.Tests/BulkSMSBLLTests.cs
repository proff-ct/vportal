using System.Collections.Generic;
using System.Linq;
using CallCenter_DAL.SMSFloat.Credit;
using CallCenter_DAL.SMSFloat.Debit.Archived;
using CallCenter_DAL.SMSFloat.Debit.Unarchived;
using NUnit.Framework;

namespace CallCenter_BLL.Tests
{
  namespace BulkSMSBLL_Functions
  {
    [TestFixture]
    public class GetLatestCreditTrxForClient
    {
      private BulkSMSBLL _bulkSMSBLL = new BulkSMSBLL();
      private readonly string corporateNo = 525201.ToString();

      [Test]
      public void Returns_latest_record_from_sacco_bulk_sms_topup_table()
      {

        BulkSMSCredit output = _bulkSMSBLL.GetLatestCreditTrxForClient(corporateNo);

        Assert.IsInstanceOf<BulkSMSCredit>(output, "Bulk SMS credit not returned!");
        Assert.IsNotNull(output.SmsCount);
      }

    }
    [TestFixture]
    public class GetCreditTrxListForClient
    {
      private BulkSMSBLL _bulkSMSBLL = new BulkSMSBLL();
      private readonly string corporateNo = 525201.ToString();

      [Test]
      public void Returns_IEnumerable_of_records_from_sacco_bulk_sms_topup_table()
      {
        var output = _bulkSMSBLL.GetCreditTrxListForClient(corporateNo);

        Assert.IsInstanceOf<IEnumerable<BulkSMSCredit>>(
          output, "IEnumerable Bulk SMS credit not returned!");
        Assert.GreaterOrEqual(output.Count(), 1, "List of records less than zero!");
      }
    }

    [TestFixture]
    public class GetUnarchivedDebitTrxListForClient
    {
      private BulkSMSBLL _bulkSMSBLL = new BulkSMSBLL();
      private readonly string corporateNo = 525201.ToString();

      [Test]
      public void Returns_IEnumerable_of_records_from_unarchived_transactions_table()
      {

        var output = _bulkSMSBLL.GetUnarchivedDebitTrxListForClient(corporateNo);

        Assert.IsInstanceOf<IEnumerable<UnarchivedBulkSMSDebit>>(output, "IEnumerable not returned!");
        Assert.GreaterOrEqual(output.Count(), 2, "Possible error: Less than 2 items returned!");
      }
    }
    [TestFixture]
    public class GetArchivedDebitTrxListForClient
    {
      private BulkSMSBLL _bulkSMSBLL = new BulkSMSBLL();
      private readonly string corporateNo = 525201.ToString();

      [Test]
      public void Returns_IEnumerable_of_records_from_archived_debit_transactions()
      {

        var output = _bulkSMSBLL.GetArchivedDebitTrxListForClient(corporateNo);

        Assert.IsInstanceOf<IEnumerable<ArchivedBulkSMSDebit>>(output, "IEnumerable not returned!");
        Assert.GreaterOrEqual(output.Count(), 2, "Possible error: Less than 2 items returned!");
      }
    }

  }

}
