using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallCenter_DAL;
using NUnit.Framework;

namespace CallCenter_BLL.Tests
{
  namespace GuarantorsBLL_Functions
  {
    [TestFixture]
    public class GetGuarantorsForLoan
    {
      GuarantorsBLL guarantorsBLL = new GuarantorsBLL();
      private dynamic _enGuarantors;
      [Test]
      public void returns_enumerable_of_guarantors_for_specified_loan_session()
      {
        string loanSessionID = 6064644092384.ToString();
        _enGuarantors = guarantorsBLL.GetGuarantorsForLoan(loanSessionID);

        Assert.IsInstanceOf<IEnumerable>(_enGuarantors);
        foreach (Guarantors guarantor in _enGuarantors)
        {
          Assert.AreEqual(guarantor.Session, loanSessionID, "Session ID Mismatch!");
        }
      }
      [Test]
      public void returns_empty_resultset_if_no_guarantors()
      {
        string loanSessionID = "Me no exists!";
        _enGuarantors = guarantorsBLL.GetGuarantorsForLoan(loanSessionID);
        Assert.IsEmpty(_enGuarantors);
      }
    }
    [TestFixture]
    public class GetGuarantorsForManyLoans
    {
      GuarantorsBLL guarantorsBLL = new GuarantorsBLL();
      private dynamic _enGuarantors;

      [Test]
      public void returns_enumerable_of_guarantors_for_specified_loan_sessions()
      {
        string[] loanSessionIDs = new string[]
        {
          6064644092384.ToString(),
          6818643809052.ToString(),
          9342642271582.ToString()
        };
        _enGuarantors = guarantorsBLL.GetGuarantorsForManyLoans(loanSessionIDs);

        Assert.IsInstanceOf<IEnumerable>(_enGuarantors);
        //foreach (string sessionID in loanSessionIDs)
        //{
        //  Assert.Contains(sessionID, _enGuarantors, "Session ID Mismatch!");
        //}
      }
    }
  }

}
