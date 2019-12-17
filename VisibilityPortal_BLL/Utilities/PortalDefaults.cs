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
        public static string PASSWORD_DEFAULT
        {
            get
            {
                //Generate Random Password
                string password = Membership.GeneratePassword(12, 1);
                return password;
            }
        

        }
    }

  }
}
