using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.PortalSecurity
{
  public static class PortalSetupSecurityTokens
  {
    public static string EncryptionSalt => "Chumvi cha kuongezea lather";
  }

  public static class MSACCOAPI
  {
    public const int MaxCredentialLength = 16;
    public const string FILLER_CHAR = "l";
  }
}
