using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MSacco_BLL.Models;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using Dapper;
using MSacco_Dataspecs.HardCoded;

namespace MSacco_BLL
{
  public class MSaccoSalaryAdvanceBLL
  {
    private readonly string _tblMSaccoSalaryAdvance = MSaccoSalaryAdvance.DBTableName;
    public IEnumerable<MSaccoSalaryAdvance> GetMSaccoSalaryAdvanceListForClient(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      if (paginate)
      {
        query = $@"SELECT * FROM {_tblMSaccoSalaryAdvance} 
          WHERE [Corporate No]=@CorporateNo
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMSaccoSalaryAdvance}
          WHERE [Corporate No]=@CorporateNo
          ";

        qryParams.Add("PageSize", pagingParams.PageSize);
        qryParams.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (var results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
          {
            IEnumerable<MSaccoSalaryAdvance> loans = results.Read<MSaccoSalaryAdvance>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              (decimal)totalLoanRecords / (decimal)pagingParams.PageSize);
            return loans;
          }
        }
      }
      else
      {
        query = $@"SELECT * FROM {_tblMSaccoSalaryAdvance} WHERE [Corporate No]=@CorporateNo ORDER BY [Entry No] DESC";
        return new DapperORM().QueryGetList<MSaccoSalaryAdvance>(query, qryParams);
      }

    }
    public IEnumerable<LoansPlusGuarantors> GetLoansListWithGuarantorsForClient(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {

      IEnumerable<MSaccoSalaryAdvance> loans = (paginate) ?
      GetMSaccoSalaryAdvanceListForClient(corporateNo, out lastPage, paginate, pagingParams)
      : GetMSaccoSalaryAdvanceListForClient(corporateNo, out lastPage);

      List<LoansPlusGuarantors> loansPlusGuarantors = new List<LoansPlusGuarantors>();

      GuarantorsBLL guarantorsBLL = new GuarantorsBLL();
      string tblGuarantors = Guarantors.DBTableName;

      IEnumerable<string> guaranteedLoanSessions = Enumerable.Empty<string>();

      // 13Jul2020_1224 temporary hack for the case of Elimu
      guaranteedLoanSessions = loans
        .Where(l => l.No_of_Guarantors > 0 || (l.Corporate_No.Equals(($"{(int)CorporateNumbers.Elimu}")) && Loans.LoanTypesWithGuarantors.Elimu.Contains(l.Loan_type)))
        .Select(l => l.SESSION_ID);
      IEnumerable<Guarantors> loanGuarantors = (guaranteedLoanSessions.Count() > 0) ?
       guarantorsBLL.GetGuarantorsForManyLoans(guaranteedLoanSessions)
       : Enumerable.Empty<Guarantors>();

      // for each loan, create a LoanPlusGuarantors object
      // how?
      // if g > 0, load the guarantors from the db
      // create new object and pass in the guarantors where if g > 0 else g == null
      foreach (MSaccoSalaryAdvance loan in loans)
      {
        LoansPlusGuarantors loanPlusGuarantors = Mapper.Map<LoansPlusGuarantors>(loan);
        loanPlusGuarantors.Guarantors = loanGuarantors.Where(g => g.Session == loan.SESSION_ID).ToList();

        // include the count for Elimu
        if (loan.Corporate_No.Equals($"{(int)CorporateNumbers.Elimu}"))
        {
          loanPlusGuarantors.No_of_Guarantors = loanGuarantors.Where(g => g.Session == loan.SESSION_ID).Count();
        }

        loansPlusGuarantors.Add(loanPlusGuarantors);
      }
      return loansPlusGuarantors.AsEnumerable();
    }

    public IEnumerable<MSaccoSalaryAdvance> GetClientMSaccoSalaryAdvanceListForToday(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      if (paginate)
      {
        query = $@"SELECT * FROM {_tblMSaccoSalaryAdvance} 
          WHERE [Corporate No]=@CorporateNo
          AND datediff(dd, [Transaction Date], getdate()) = 0
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMSaccoSalaryAdvance}
          WHERE [Corporate No]=@CorporateNo
          ";

        qryParams.Add("PageSize", pagingParams.PageSize);
        qryParams.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (var results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
          {
            IEnumerable<MSaccoSalaryAdvance> loans = results.Read<MSaccoSalaryAdvance>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              (decimal)totalLoanRecords / (decimal)pagingParams.PageSize);
            return loans;
          }
        }
      }
      else
      {
        query = $@"SELECT * FROM {_tblMSaccoSalaryAdvance} 
                  WHERE [Corporate No]=@CorporateNo
                  AND datediff(dd, [Transaction Date], getdate()) = 0";
        return new DapperORM().QueryGetList<MSaccoSalaryAdvance>(query, qryParams);
      }

    }

  }

}
