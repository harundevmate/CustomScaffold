using System;
using System.Collections.Generic;
using Shared;

namespace BusinessCore
{
    public partial class UnitMeasure : BaseEntity
    {
        public UnitMeasure()
        {
            Items = new HashSet<Item>();
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
