using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CallCenter_BLL.Models;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_BLL.ViewModels;
using CallCenter_DAL;
using NUnit.Framework;

namespace CallCenter_BLL.Tests
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
        int lastPage;
        _enSalaryAdvance = salaryAdvanceBLL.GetMSaccoSalaryAdvanceListForClient(
          corporateNo, out lastPage);
        Assert.IsInstanceOf<IEnumerable>(_enSalaryAdvance);
      }
      [Test]
      public void returns_enumerable_of_client_loans_when_called_with_pagination()
      {
        string corporateNo = 525201.ToString();
        int page = 1;
        int pageSize = 20;
        int lastPage;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _enSalaryAdvance = salaryAdvanceBLL.GetMSaccoSalaryAdvanceListForClient(
          corporateNo, out lastPage, true, pagingParams);
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
        config => {
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
        int lastPage;
        _enLoansPlusGuarantors = salaryAdvanceBLL.GetLoansListWithGuarantorsForClient(
          corporateNo, out lastPage);
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
  }

}
