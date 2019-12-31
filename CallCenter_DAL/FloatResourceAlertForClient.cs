namespace CallCenter_DAL
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("FloatResourceAlertForClient")]
  public partial class FloatResourceAlertForClient
  {
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(256)]
    public string ClientCorporateNo { get; set; }
    [Required]
    [StringLength(256)]
    public string FloatResourceId { get; set; }
    [Required]
    public int AlertTypeId { get; set; }
    [Required]
    public string Threshold { get; set; }
    [Required]
    public string TriggerCondition { get; set; }

    public string CreatedBy { get; set; }
    [Column(TypeName = "datetime2")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedOn { get; private set; }
    public string ModifiedBy { get; set; }
    [Column(TypeName = "datetime2")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime ModifiedOn { get; private set; }
  }
}
