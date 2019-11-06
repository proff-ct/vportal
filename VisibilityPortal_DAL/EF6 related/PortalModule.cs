namespace VisibilityPortal_DAL
{
  public partial class PortalModule
  {
    public enum modules
    {
      AgencyBanking,
      MSacco,
      CallCenter
    };

    public static class AgencyBankingModule
    {
      public static string moduleName => modules.AgencyBanking.ToString();
      public static string routePrefix => "AgencyBanking";
    }
    public static class MSaccoModule
    {
      public static string moduleName => modules.MSacco.ToString();
      public static string routePrefix => "MSacco";
    }
    public static class CallCenterModule
    {
      public static string moduleName => modules.CallCenter.ToString();
      public static string routePrefix => "CallCenter";
    }

  }
  
}
