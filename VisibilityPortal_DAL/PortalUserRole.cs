namespace VisibilityPortal_DAL
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("PortalUserRole")]
  public partial class PortalUserRole
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(50)]
    public string ClientModuleId { get; set; }

    [StringLength(50)]
    public string UserId { get; set; }

    [StringLength(50)]
    public string AspRoleId { get; set; }
    [StringLength(50)]
    public string AspRoleName { get; set; }
    public string CreatedBy { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedOn { get; private set; }
    public string ModifiedBy { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime ModifiedOn { get; private set; }
    public bool IsEnabled { get; set; }
  }
}
