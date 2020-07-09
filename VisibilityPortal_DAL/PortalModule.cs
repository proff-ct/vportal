namespace VisibilityPortal_DAL
{
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using VisibilityPortal_Dataspecs.Models;

  [Table("PortalModule")]
  public partial class PortalModule : IPortalModule
  {
    [Key]
    [StringLength(256)]
    public string ModuleName { get; set; }

    [StringLength(256)]
    public string CoreTecProductName { get; set; }
    [StringLength(256)]
    public string RoutePrefix { get; set; }
  }
}
