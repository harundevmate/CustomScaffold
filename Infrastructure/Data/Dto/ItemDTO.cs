using System;
using System.Collections.Generic;
using Shared;
using System.ComponentModel.DataAnnotations;
namespace Infrastructure
{
  public partial class ItemDTO
  {
      public Guid Id { get; set; }
      public string Code { get; set; }
      public string Name { get; set; }
      public Guid UnitMeasureId { get; set; }
      public string Description { get; set; }
      public string CreatedBy { get; set; }
      public DateTime CreatedAt { get; set; }
      public string ModifiedBy { get; set; }
      public DateTime? ModifiedAt { get; set; }
      public string DeletedBy { get; set; }
      public DateTime? DeletedAt { get; set; }
      //public virtual UnitMeasureDTO UnitMeasure { get; set; }
  }

  public partial class ItemDTOWithDetail : BaseEntity
  {
      //public virtual UnitMeasureDTO UnitMeasure { get; set; }
  }
}
