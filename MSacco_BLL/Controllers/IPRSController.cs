using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MSacco_BLL.CustomFilters;
using MSacco_BLL.MSSQLOperators;
using MSacco_BLL.ViewModels;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.IPRS;
using MSacco_Dataspecs.Security;
using Newtonsoft.Json;
using Utilities.PortalApplicationParams;

namespace MSacco_BLL.Controllers
{
  [RequireActiveUserSession]
  public class IPRSController : Controller
  {
    private IBL_IPRS _iprsBLL = new IPRSBLL();
    // GET: IPRS
    public ActionResult Index()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"]; 
      return View();
    }
    public ActionResult WauminiMsaccoRegistration()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
      return View();
    }
    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult SearchIPRSRecord(string clientCorporateNo, string nationalIDNo, string phoneNo)
    {
      ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
      if (userParams == null || string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(nationalIDNo))
      {
        return null;
      }
      if (phoneNo.Length < 10 )
      {
        throw new ApplicationException("Invalid Phone Number specified");
      }

      // the flow:
      // 1. get the pagination parameters
      // 2. pass the pagination parameters to the bll function
      // 3. retrieve the data from the bll function


      //PaginationParameters pagingParams = new PaginationParameters(
      //  int.Parse(page), int.Parse(size), null);
      List<dynamic> returnData = new List<dynamic>();

      IIPRS_Record iprsRecord = _iprsBLL.PerformIPRSLookup(clientCorporateNo, nationalIDNo, phoneNo);
      dynamic rec = Mapper.Map<WauminiVirtualRegistrationIPRS, WauminiIPRSLookupViewModel>((WauminiVirtualRegistrationIPRS)iprsRecord);
      returnData.Add(rec);

      return Json(
        APICommunication.Encrypt(
            JsonConvert.SerializeObject(returnData.ToArray()),
            new MSACCO_AES(userParams.APIAuthID, userParams.APIToken)),
        JsonRequestBehavior.AllowGet);

    }
    
    #endregion

  }
}
