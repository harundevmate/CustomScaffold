using System;
using System.Collections.Generic;
using Shared;

namespace ScaffoldHandler.Models
{
    public partial class Item : EntityBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid UnitMeasureId { get; set; }

        public virtual UnitMeasure UnitMeasure { get; set; }
    }
}
