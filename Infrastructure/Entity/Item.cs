using System;
using System.Collections.Generic;
using Shared;

namespace Infrastructure
{
    public partial class Item : BaseEntity
    {
        public string Name { get; set; }
        public string UnitMeasureId { get; set; }

        public virtual UnitMeasure UnitMeasure { get; set; }
    }
}
