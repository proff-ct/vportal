﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSacco_Dataspecs
{
  public static class MSACCOToolbox
  {
    public static string FormatPhoneNumberForSQLLookup(string phoneNo)
    {
      return string.Format("'{0}'", phoneNo);
    }

    public static string ParsePhoneNo(string TelephoneNo)
    {
      string unwantedChar = Regex.Escape("+();'_-");
      string pattern = string.Format("[{0}]", unwantedChar);

      TelephoneNo = Regex.Replace(TelephoneNo, pattern, "");

      if (!IsDigitsOnly(TelephoneNo))
      {
        throw new ArgumentException("Invalid phone number value");
      }

      return string.Format("+{0}", TelephoneNo);
    }

    public static bool IsDigitsOnly(string s)
    {
      if (s == null || s == "")
      {
        return false;
      }

      for (int i = 0; i < s.Length; i++)
      {
        if (s[i] < '0' || s[i] > '9')
        {
          return false;
        }
      }

      return true;
    }
  }
}
