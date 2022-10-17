using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Shared
{
  public partial class UnitMeasureDTO
  {
      public string Id { get; set; }
      [Required]
      public string Name { get; set; }
      public string Description { get; set; }
      public string CreatedBy { get; set; }
      public DateTime CreatedAt { get; set; }
      public string ModifiedBy { get; set; }
      public DateTime? ModifiedAt { get; set; }
      public string DeletedBy { get; set; }
      public DateTime? DeletedAt { get; set; }
      //public virtual ICollection<ItemDTO>Items { get; set; }
  }

  public partial class UnitMeasureDTOWithDetail : BaseEntity
  {
      //public virtual ICollection<ItemDTO>Items { get; set; }
  }
}
