using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgencyBanking_UI
{
    public class Currency
    {
        /// <summary>
        /// Adds commas to a string amount
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static String ToMoney(string money)
        {
           double temp = Double.Parse(money);
           return temp.ToString("N");
        }
    }
}