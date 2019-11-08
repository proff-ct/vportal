namespace VisibilityPortal_DAL
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("PortalModuleForClient")]
  public partial class PortalModuleForClient
  {
    [Key]
    [StringLength(50)]
    public string ClientModuleId { get; set; }

    [Required]
    [StringLength(50)]
    public string ClientCorporateNo { get; set; }

    [Required]
    [StringLength(256)]
    public string PortalModuleName { get; set; }

    public bool IsActivated { get; set; }
    public string CreatedBy { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedOn { get; private set; }
    public string ModifiedBy { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime ModifiedOn { get; private set; }
  }
}
