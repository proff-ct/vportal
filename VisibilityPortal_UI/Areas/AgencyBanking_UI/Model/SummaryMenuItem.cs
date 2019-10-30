using System;
using System.Collections.Generic;

namespace AgencyBanking_UI.Model
{
    /// <summary>
    /// This is used to display data into the Summary page
    /// </summary>
   public  class SummaryMenuItem
    {
        public string Title { get; }
        public string Imagelink { get; }
        public string FieldName { get; }
        public String Amount { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">The String to Display on the card</param>
        /// <param name="imagelink">The Image Url</param>
        /// <param name="fieldname">The Key in the Dictonary</param>
        /// <param name="data">The Dictonary Data</param>
        public SummaryMenuItem(string title, string imagelink, string fieldname,Dictionary<string,string> data)
        {
            Title = title;
            Imagelink = imagelink;
            FieldName = fieldname;
            try
            {
                Amount = Currency.ToMoney(data[fieldname]);
            }
            catch
            {
                Amount = "0";
            }
          
        }

    }
}
