using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TobeRemove
{
    public class RemoveMe
    {
        public Count GetCount(ICollection<WhatEver> data)
        {
            var impressionCount = data.Where(x => x == WhatEver.impression).Count();
            return new Count { ImpressionCount = impressionCount, ClickCount = (data.Count() - impressionCount) };
        }
    }

    public enum WhatEver
    {
        impression, click
    }

    public class Count
    {
        public double ImpressionCount { get; set; }
        public double ClickCount { get; set; }
    }
}
