using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallCenter_Dataspecs.MSSQLOperators;

namespace CallCenter_Dataspecs.USSDRequests
{
  namespace Models
  {
    public interface IUSSDRequest
    {
      int Entry_No { get; set; }
      string User_Input { get; set; }
      string Reply { get; set; }
      string Session_ID { get; set; }
      DateTime? DateTime { get; set; }
      string Telephone_No { get; set; }
      string Corporate_No { get; set; }
    }

    public interface IUSSD_Request_ViewModel
    {
      int Entry_No { get; set; }
      string User_Input { get; set; }
      string MSACCOResponse { get; set; }
      string Session_ID { get; set; }
      DateTime? DateTime { get; set; }
      string Telephone_No { get; set; }
      string Corporate_No { get; set; }
    }
    public class USSDRequestVM : IUSSD_Request_ViewModel
    {
      public int Entry_No { get; set; }
      public string User_Input { get; set; }
      public string MSACCOResponse { get; set; }
      public string Session_ID { get; set; }
      public DateTime? DateTime { get; set; }
      public string Telephone_No { get; set; }
      public string Corporate_No { get; set; }
    }
  }

  namespace Functions
  {
    public interface IBL_USSDRequestLog
    {
      IEnumerable<Models.IUSSD_Request_ViewModel> GetUSSDRequestLogsForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);

      IEnumerable<Models.IUSSD_Request_ViewModel> GetMemberUSSDRequestLogForClient(string corporateNo, string memberPhoneNo);
    }

    public static class USSDRequests_BL
    {
      public static string USSD_RESPONSE_DELIMITER => "*";
      public static string ExtractUserResponseFromUSSDString(string ussdString, bool hasPIN = false)
      {
        string[] currentResponse = ussdString.Split(new string[] { USSD_RESPONSE_DELIMITER }, StringSplitOptions.None);
        int responseLength = currentResponse.Length;

        if (hasPIN && currentResponse[responseLength - 1].Length == 4)
        {
          currentResponse[responseLength - 1] = "PIN";
        }

        return currentResponse[responseLength - 1];
      }
      public static IEnumerable<Models.IUSSD_Request_ViewModel> ParseUSSDRequests(IEnumerable<Models.IUSSDRequest> loggedRequests)
      {
        List<Models.IUSSD_Request_ViewModel> parsedUSSDRequests = new List<Models.USSDRequestVM>().OfType<Models.IUSSD_Request_ViewModel>().ToList();

        loggedRequests.GroupBy(requests => requests.Session_ID).ToList().ForEach(ussdSession =>
        {

          Models.IUSSDRequest[] sessionPrompts = ussdSession.OrderBy(session => session.Entry_No).ToArray();

          for (int i = 0; i < sessionPrompts.Length; i++)
          {
            bool hasPIN = false;
            if (i == 0 || sessionPrompts[i].Reply.ToUpper().Contains("PIN"))
            {
              hasPIN = true;
            }
            parsedUSSDRequests.Add(new Models.USSDRequestVM
            {
              Session_ID = sessionPrompts[i].Session_ID,
              Entry_No = sessionPrompts[i].Entry_No,
              Corporate_No = sessionPrompts[i].Corporate_No,
              Telephone_No = sessionPrompts[i].Telephone_No,
              MSACCOResponse = sessionPrompts[i].Reply,
              DateTime = sessionPrompts[i].DateTime,
              User_Input = (i == (sessionPrompts.Length - 1)) ?
              string.Empty : ExtractUserResponseFromUSSDString(sessionPrompts[i + 1].User_Input, hasPIN)
            });
          }
        });

        return parsedUSSDRequests;
      }
    }
  }
}
