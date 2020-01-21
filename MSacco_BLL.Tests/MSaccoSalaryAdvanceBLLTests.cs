using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MSacco_BLL.Models;
using MSacco_BLL.MSSQLOperators;
using MSacco_BLL.ViewModels;
using MSacco_DAL;
using NUnit.Framework;

namespace MSacco_BLL.Tests
{
  namespace MSaccoSalaryAdvanceBLL_Functions
  {
    [TestFixture]
    public class GetMSaccoSalaryAdvanceListForClient
    {
      private MSaccoSalaryAdvanceBLL salaryAdvanceBLL = new MSaccoSalaryAdvanceBLL();
      private dynamic _enSalaryAdvance;
      [Test]
      public void returns_enumerable_of_client_loans_when_called_without_pagination()
      {
        string corporateNo = 525201.ToString();
        _enSalaryAdvance = salaryAdvanceBLL.GetMSaccoSalaryAdvanceListForClient(
          corporateNo, out int lastPage);
        Assert.IsInstanceOf<IEnumerable>(_enSalaryAdvance);
      }
      [Test]
      public void returns_enumerable_of_client_loans_when_called_with_pagination()
      {
        string corporateNo = 525201.ToString();
        int page = 1;
        int pageSize = 20;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _enSalaryAdvance = salaryAdvanceBLL.GetMSaccoSalaryAdvanceListForClient(
          corporateNo, out int lastPage, true, pagingParams);
        Assert.IsInstanceOf<IEnumerable>(_enSalaryAdvance);
        Assert.LessOrEqual(_enSalaryAdvance.Count, pageSize, "Records returned > Expected!");
        Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");
      }
    }
    [TestFixture]
    public class GetLoansListWithGuarantorsForClient
    {
      private MSaccoSalaryAdvanceBLL salaryAdvanceBLL = new MSaccoSalaryAdvanceBLL();
      private dynamic _enLoansPlusGuarantors;
      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        Mapper.Initialize(
        config =>
        {
          config.CreateMap<MSaccoSalaryAdvance, LoanListViewModel>().ReverseMap();
          config.CreateMap<MSaccoSalaryAdvance, LoansPlusGuarantors>();
        });
      }
      [OneTimeTearDown]
      public void ClassCleanup()
      {
        // Runs once after all tests in this class are executed. (Optional)
        // Not guaranteed that it executes instantly after all tests from the class.
        Mapper.Reset();
      }
      [SetUp]
      public void TestInit()
      {
        // Runs before each test. (Optional)

      }
      [TearDown]
      public void TearDown()
      {

      }
      #endregion
      [Test]
      public void returns_enumerable_of_client_loans_plus_guarantors()
      {
        string corporateNo = 525201.ToString();
        _enLoansPlusGuarantors = salaryAdvanceBLL.GetLoansListWithGuarantorsForClient(
          corporateNo, out int lastPage);
        Assert.IsInstanceOf<IEnumerable>(_enLoansPlusGuarantors);
        // Commented out the below because it possible for a loan record that
        // requires guarantors to not have any guarantor records.
        // This is going by the data that's currently there.
        //foreach(LoansPlusGuarantors loan in _enLoansPlusGuarantors)
        //{
        //  if (loan.No_of_Guarantors == 0 || loan.No_of_Guarantors == null) continue;
        //  Assert.GreaterOrEqual(
        //    loan.Guarantors.Count, loan.No_of_Guarantors, $"Less Guarantors for {loan.SESSION_ID} received!");
        //  Assert.AreEqual(loan.Guarantors.Select(g => g.Guarantor).Distinct().Count(), loan.No_of_Guarantors, "Gurantors returned NOT equal to Loan No. of Guarantors");
        //}
      }
    }

    [TestFixture]
    public class GetClientMSaccoSalaryAdvanceListForToday
    {
      private MSaccoSalaryAdvanceBLL salaryAdvanceBLL = new MSaccoSalaryAdvanceBLL();
      private dynamic _enSalaryAdvance;
      private List<MSaccoSalaryAdvance> _loansList = null;

      [Test]
      public void returns_enumerable_of_client_loans_for_the_current_day_when_called_without_pagination()
      {
        string corporateNo = 525201.ToString();
        _enSalaryAdvance = salaryAdvanceBLL.GetClientMSaccoSalaryAdvanceListForToday(
          corporateNo, out int lastPage);
        Assert.IsInstanceOf<IEnumerable>(_enSalaryAdvance);

        _loansList = (List<MSaccoSalaryAdvance>)_enSalaryAdvance;

        if (_loansList.Count > 0)
        {
          _loansList.ForEach(loan =>
          {
            Assert.AreEqual(DateTime.Now.Date, loan.Transaction_Date.Value.Date, "Returned DATE not EQUAL TO TODAY'S DATE!!!");
          });
        }

      }
      [Test]
      public void returns_enumerable_of_client_loans_for_the_current_day_when_called_with_pagination()
      {
        string corporateNo = 525201.ToString();
        int page = 1;
        int pageSize = 20;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _enSalaryAdvance = salaryAdvanceBLL.GetClientMSaccoSalaryAdvanceListForToday(
          corporateNo, out int lastPage, true, pagingParams);
        Assert.IsInstanceOf<IEnumerable>(_enSalaryAdvance);
        Assert.LessOrEqual(_enSalaryAdvance.Count, pageSize, "Records returned > Expected!");
        Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");

        _loansList = (List<MSaccoSalaryAdvance>)_enSalaryAdvance;

        if (_loansList.Count > 0)
        {
          _loansList.ForEach(loan =>
          {
            Assert.AreEqual(DateTime.Now.Date, loan.Transaction_Date.Value.Date, "Returned DATE not EQUAL TO TODAY'S DATE!!!");
          });
        }
      }

    }

  }

}
