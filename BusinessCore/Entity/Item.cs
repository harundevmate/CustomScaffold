using System;
using System.Collections.Generic;
using Shared;

namespace ScaffoldHandler
{
    public partial class Item : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid UnitMeasureId { get; set; }

        public virtual UnitMeasure UnitMeasure { get; set; }
    }
}
