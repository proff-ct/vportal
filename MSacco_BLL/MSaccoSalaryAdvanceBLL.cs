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

namespace MSacco_BLL
{
  public class MSaccoSalaryAdvanceBLL
  {
    private string _query;
    private readonly string _tblMSaccoSalaryAdvance = MSaccoSalaryAdvance.DBTableName;
    public IEnumerable<MSaccoSalaryAdvance> GetMSaccoSalaryAdvanceListForClient(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblMSaccoSalaryAdvance} 
          WHERE [Corporate No]='{corporateNo}'
          ORDER BY [Entry No]
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMSaccoSalaryAdvance}
          WHERE [Corporate No]='{corporateNo}'
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (var results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
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
        _query = $@"SELECT * FROM {_tblMSaccoSalaryAdvance} WHERE [Corporate No]='{corporateNo}'";
        return new DapperORM().QueryGetList<MSaccoSalaryAdvance>(_query);
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

      IEnumerable<string> guaranteedLoanSessions = loans.Where(l => l.No_of_Guarantors > 0)
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

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblMSaccoSalaryAdvance} 
          WHERE [Corporate No]='{corporateNo}'
          AND datediff(dd, [Transaction Date], getdate()) = 0
          ORDER BY [Entry No]
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMSaccoSalaryAdvance}
          WHERE [Corporate No]='{corporateNo}'
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (var results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
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
        _query = $@"SELECT * FROM {_tblMSaccoSalaryAdvance} 
                  WHERE [Corporate No]='{corporateNo}'
                  AND datediff(dd, [Transaction Date], getdate()) = 0
                  ";
        return new DapperORM().QueryGetList<MSaccoSalaryAdvance>(_query);
      }

    }

  }

}
