using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MSacco_BLL.Models;
using MSacco_BLL.MSSQLOperators;
using MSacco_BLL.ViewModels;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Functions;
using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Models;
using NUnit.Framework;

namespace MSacco_BLL.Tests
{
  namespace MsaccoBankTransferBLL_Functions
  {
    [TestFixture]
    public class IsClientUsingCoretecFloat
    {
      private IBL_BankTransfer bankTransferBLL = new MsaccoBankTransferBLL(new SaccoBLL());
      private dynamic _usingCoretecFloat;

      [Test]
      public void returns_true_when_sacco_is_using_coretec_pesalink_float()
      {
        string corporateNo = 911201.ToString();
        _usingCoretecFloat = bankTransferBLL.IsClientUsingCoretecFloat(corporateNo);
        Assert.IsTrue(_usingCoretecFloat, "Returned false where true expected!!");
      }

      [Test]
      public void returns_false_when_sacco_is_not_using_coretec_pesalink_float()
      {
        string corporateNo = 525201.ToString();
        _usingCoretecFloat = bankTransferBLL.IsClientUsingCoretecFloat(corporateNo);
        Assert.IsFalse(_usingCoretecFloat);
      }

      [Test]
      public void throws_ArgumentException_when_sacco_not_found_by_corporateNo()
      {
        string corporateNo = "";
        Assert.Throws<ArgumentException>(
          () => bankTransferBLL.IsClientUsingCoretecFloat(corporateNo),
          "Argument Exception not thrown!");
      }
    }
    [TestFixture]
    public class GetClientFloat
    {
      private IBL_BankTransfer bankTransferBLL = new MsaccoBankTransferBLL(new SaccoBLL());
      private dynamic _output;

      [Test]
      public void returns_client_float_balance_record()
      {
        string corporateNo = 892801.ToString();
        _output = bankTransferBLL.GetClientFloat(corporateNo);
        Assert.IsInstanceOf<IClientBankTransferFloat>(_output, "Expected Client float record not returned");
      }
      [Test]
      public void throws_ArgumentNullException_when_pesalink_float_balance_record_not_found()
      {
        string corporateNo = "911201";
        
        Assert.Throws<ArgumentNullException>(
          () => bankTransferBLL.GetClientFloat(corporateNo),
          "Argument Null Exception not thrown!");
      }

      [Test]
      public void throws_ArgumentException_when_sacco_not_found_by_corporateNo()
      {
        string corporateNo = "";
        Assert.Throws<ArgumentException>(
          () => bankTransferBLL.GetClientFloat(corporateNo),
          "Argument Exception not thrown!");
      }
    }
    //[TestFixture]
    //public class GetLoansListWithGuarantorsForClient
    //{
    //  private MsaccoBankTransferBLL bankTransferBLL = new MsaccoBankTransferBLL();
    //  private dynamic _enLoansPlusGuarantors;
    //  #region TestPreparation

    //  [OneTimeSetUp]
    //  public void ClassInit()
    //  {
    //    // Executes once for the test class. (Optional) 
    //    Mapper.Initialize(
    //    config =>
    //    {
    //      config.CreateMap<MSaccoBankTransfer, LoanListViewModel>().ReverseMap();
    //      config.CreateMap<MSaccoBankTransfer, LoansPlusGuarantors>();
    //    });
    //  }
    //  [OneTimeTearDown]
    //  public void ClassCleanup()
    //  {
    //    // Runs once after all tests in this class are executed. (Optional)
    //    // Not guaranteed that it executes instantly after all tests from the class.
    //    Mapper.Reset();
    //  }
    //  [SetUp]
    //  public void TestInit()
    //  {
    //    // Runs before each test. (Optional)

    //  }
    //  [TearDown]
    //  public void TearDown()
    //  {

    //  }
    //  #endregion
    //  [Test]
    //  public void returns_enumerable_of_client_loans_plus_guarantors()
    //  {
    //    string corporateNo = 525201.ToString();
    //    _enLoansPlusGuarantors = bankTransferBLL.GetLoansListWithGuarantorsForClient(
    //      corporateNo, out int lastPage);
    //    Assert.IsInstanceOf<IEnumerable>(_enLoansPlusGuarantors);
    //    // Commented out the below because it possible for a loan record that
    //    // requires guarantors to not have any guarantor records.
    //    // This is going by the data that's currently there.
    //    //foreach(LoansPlusGuarantors loan in _enLoansPlusGuarantors)
    //    //{
    //    //  if (loan.No_of_Guarantors == 0 || loan.No_of_Guarantors == null) continue;
    //    //  Assert.GreaterOrEqual(
    //    //    loan.Guarantors.Count, loan.No_of_Guarantors, $"Less Guarantors for {loan.SESSION_ID} received!");
    //    //  Assert.AreEqual(loan.Guarantors.Select(g => g.Guarantor).Distinct().Count(), loan.No_of_Guarantors, "Gurantors returned NOT equal to Loan No. of Guarantors");
    //    //}
    //  }
    //}

    //[TestFixture]
    //public class GetClientMSaccoBankTransferListForToday
    //{
    //  private MsaccoBankTransferBLL bankTransferBLL = new MsaccoBankTransferBLL();
    //  private dynamic _enBankTransfer;
    //  private List<MSaccoBankTransfer> _loansList = null;

    //  [Test]
    //  public void returns_enumerable_of_client_loans_for_the_current_day_when_called_without_pagination()
    //  {
    //    string corporateNo = 525201.ToString();
    //    _enBankTransfer = bankTransferBLL.GetClientMSaccoBankTransferListForToday(
    //      corporateNo, out int lastPage);
    //    Assert.IsInstanceOf<IEnumerable>(_enBankTransfer);

    //    _loansList = (List<MSaccoBankTransfer>)_enBankTransfer;

    //    if (_loansList.Count > 0)
    //    {
    //      _loansList.ForEach(loan =>
    //      {
    //        Assert.AreEqual(DateTime.Now.Date, loan.Transaction_Date.Value.Date, "Returned DATE not EQUAL TO TODAY'S DATE!!!");
    //      });
    //    }

    //  }
    //  [Test]
    //  public void returns_enumerable_of_client_loans_for_the_current_day_when_called_with_pagination()
    //  {
    //    string corporateNo = 525201.ToString();
    //    int page = 1;
    //    int pageSize = 20;
    //    PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

    //    _enBankTransfer = bankTransferBLL.GetClientMSaccoBankTransferListForToday(
    //      corporateNo, out int lastPage, true, pagingParams);
    //    Assert.IsInstanceOf<IEnumerable>(_enBankTransfer);
    //    Assert.LessOrEqual(_enBankTransfer.Count, pageSize, "Records returned > Expected!");
    //    Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");

    //    _loansList = (List<MSaccoBankTransfer>)_enBankTransfer;

    //    if (_loansList.Count > 0)
    //    {
    //      _loansList.ForEach(loan =>
    //      {
    //        Assert.AreEqual(DateTime.Now.Date, loan.Transaction_Date.Value.Date, "Returned DATE not EQUAL TO TODAY'S DATE!!!");
    //      });
    //    }
    //  }

    //}

  }

}
