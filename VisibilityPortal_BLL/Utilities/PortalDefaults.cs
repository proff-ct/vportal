using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace VisibilityPortal_BLL.Utilities
{
  namespace PortalDefaults
  {
    public static class ApplicationUserDefaults
    {
        /// <summary>
        /// TODO: Generate Random Password
        /// </summary>
        public static string PASSWORD_DEFAULT => "User.123";
    }

  }
}
