namespace CallCenter_DAL
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("AlertType")]
  public partial class AlertType
  {
    [Key]
    public int Id { get; set; }
    [StringLength(256)]
    public string AlertName { get; set; }
    [StringLength(256)]
    public string Description { get; set; }

    public string CreatedBy { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedOn { get; private set; }
    public string ModifiedBy { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime ModifiedOn { get; private set; }
  }
}
