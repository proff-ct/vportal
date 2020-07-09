using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Utilities;
using Utilities.PortalApplicationParams;
using VisibilityPortal_DAL;
using VisibilityPortal_Dataspecs.Models;

namespace VisibilityPortal_BLL.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    IBL_PortalModule _portalModuleBLL = new PortalModuleBLL();
    IBL_CoretecClient _coretecClientBLL = new CoretecClientBLL();
    public ActionResult Index()
    {
      dynamic model = Session["ClientModuleUrls"];
      //if (model != null) ViewBag.ClientModuleUrls = model;
      //return model!=null ? View(model) : View();

      if(model != null)
      {
        ViewBag.ClientModuleUrls = model;
        return View(model);
      }
      else
      {
        try
        {
          IActiveUserParams activeUser = (ActiveUserParams)Session[ActiveUserParams.SessionVaribleName()];
          return RedirectToPortalModule(activeUser.ClientModuleId);
        }
        catch(ApplicationException ex)
        {
          
          AppLogger.LogOperationException(
            nameof(RedirectToPortalModule),
            $"Application Exception: {ex.Message}",
            null,
            ex);

          // what to do ?
          return View();
        }
        catch(Exception ex)
        {
          AppLogger.LogOperationException(
            nameof(RedirectToPortalModule),
            "Error when redirecting to portal module",
            null,
            ex);

          return RedirectToAction("SignOut", "Account");
        }
        
      }

      //return RedirectToAction("Login", "Account");
    }

    public ActionResult About()
    {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Your contact page.";

      return View();
    }

    private ActionResult RedirectToPortalModule(string clientModuleID)
    {
      IPortalModuleForClient clientModule = _coretecClientBLL.GetPortalModuleForClient(clientModuleID);
      if (string.IsNullOrEmpty(clientModuleID)) throw new ApplicationException("Client Module ID not specified");
      if (clientModule.PortalModuleName == PortalModule.AgencyBankingModule.moduleName)
      {

        return RedirectToRoute(
          PortalModule.AgencyBankingModule.defaultRoute,
          (RouteTable.Routes[PortalModule.AgencyBankingModule.defaultRoute] as Route).Defaults);
      }
      else if (clientModule.PortalModuleName == PortalModule.MSaccoModule.moduleName)
      {

        return RedirectToRoute(
          PortalModule.MSaccoModule.defaultRoute,
          (RouteTable.Routes[PortalModule.MSaccoModule.defaultRoute] as Route).Defaults);
      }
      else if (clientModule.PortalModuleName == PortalModule.CallCenterModule.moduleName)
      {
        return RedirectToRoute(
          PortalModule.CallCenterModule.defaultRoute,
          (RouteTable.Routes[PortalModule.CallCenterModule.defaultRoute] as Route).Defaults);
      }
      else
      {
        throw new ApplicationException($"Client Module ID: {clientModuleID} not mapped");
      }
    }
  }
}