﻿using System.Web;
using System.Web.Mvc;

namespace Visibility_Portal
{
  public class FilterConfig
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }
  }
}
