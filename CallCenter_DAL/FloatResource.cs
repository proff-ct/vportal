namespace CallCenter_DAL
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("FloatResource")]
  public partial class FloatResource
  {
    [Key]
    [StringLength(256)]
    public string Id { get; set; }

    [StringLength(256)]
    public string ResourceName { get; set; }

    public string CreatedBy { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedOn { get; private set; }
    public string ModifiedBy { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime ModifiedOn { get; private set; }
  }
}
