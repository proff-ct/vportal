namespace VisibilityPortal_Dataspecs.Models
{
  public interface IPortalModule
  {
    string CoreTecProductName { get; set; }
    string ModuleName { get; set; }
    string RoutePrefix { get; set; }
  }
}