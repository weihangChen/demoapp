using Helpers.ExpressionTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Helpers.Extensions;
using System.Data.Entity;
using Helpers.ExpressionTree;
using System.Reflection;

namespace Helpers.GenericModel
{
    public enum SortingOrder
    {
        Descending, Ascending
    }
    public class SortingModel
    {
        public SortingModel AsSortingModel
        {
            get
            {
                return (SortingModel)this;
            }
        }
        public SortingModel()
        {
        }
        public SortingModel(string SortingColumn)
        {
            this.SortingColumnCurrent = SortingColumn;
            this.SortingOrderCurrent = SortingOrder.Descending;
        }
        public string SortingColumnCurrent { get; set; }
        public string SortingColumnNew { get; set; }
        public bool HasBeenSorted { get; set; }
        public bool ShouldReOrder { get; set; }

        public SortingOrder SortingOrderCurrent { get; set; }



    }

}
