using Helpers.ExpressionTree;
using Helpers.ExpressionTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Helpers.Extensions;

namespace Helpers.GenericModel
{
    /// <summary>
    /// generic base model supporting paging and free text search
    /// </summary>
    public class SortingFilterModel : SortingModel
    {
        protected static string Where = "Where";
        public string PropertyNameFTS { get; set; }
        public int PageTotal { get; set; }
        public bool ShouldResetPageIndex { get; set; }
        public List<SinglePageData> SinglePageDatas { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string PropertyValueFTS { get; set; }
        public int LeftRightBuffer { get; set; }
        public SortingFilterModel AsSortingFilterModel
        {
            get
            {
                return (SortingFilterModel)this;
            }
        }

        /// <summary>
        /// set some default value for PageIndex and PageSize
        /// </summary>
        public SortingFilterModel()
        {
            PageIndex = 1;
            PageSize = 3;
            ShouldResetPageIndex = false;
            SinglePageDatas = new List<SinglePageData>();
            LeftRightBuffer = 2;
        }

    }


    public class SinglePageData
    {
        public string PageLabel { get; set; }
        public string PageId { get; set; }
        public bool IsCurrentPageSelected { get; set; }
        public string PageCssClass { get; set; }
    }
}
