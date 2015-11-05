using Helpers.ExpressionTree;
using Helpers.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Helpers.Extensions;

namespace Helpers.Service
{
    public class FilterService : IFilterService
    {
        private IExpressionBuilder ExpBuilder;
        private ISortingService SortingService;
        protected static string Where = "Where";
        public FilterService(IExpressionBuilder ExpBuilder, ISortingService SortingService)
        {
            this.ExpBuilder = ExpBuilder;
            this.SortingService = SortingService;
        }
        /// <summary>
        /// suppose to generate query like this
        /// Where(x => x.propertyA.ToLower().Contains(TargetValue.ToLower()));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="MemberName"></param>
        /// <param name="TargetValue"></param>
        /// <returns></returns>
        public IQueryable<T> AppendQueryWithContains<T>(string MemberName, string TargetValue, IQueryable<T> queryableData, SortingFilterModel Model)
        {
            //create chainedrules at two-level deep, tolower as level1 and contains as level2
            var root = new ChainedRule(MemberName, Operator.ToLower, null);
            root.ChildRule = new ChainedRule(MemberName, Operator.Contains, TargetValue.ToLower());
            ParameterExpression p = Expression.Parameter(typeof(T), "x");
            Expression right = ExpBuilder.EvaluateChainedRule<T>(root, p);
            var whereExp = Expression.Lambda<Func<T, bool>>(right, new ParameterExpression[] { p });

            var whereMethodExpression = ExpBuilder.BuildMethodExpression(queryableData, Where, whereExp, typeArguments: new Type[] { queryableData.ElementType });


            IQueryable<T> results = queryableData.EvalQueryMethodCallExpression(whereMethodExpression);
            //ShouldResetPageIndex value is set at the front end, when freetext search is triggered, always get the content from pageindex1
            if (Model.ShouldResetPageIndex)
            {
                ResetPageIndex(Model);
            }
            return results;
        }

        protected void ResetPageIndex(SortingFilterModel Model)
        {
            Model.PageIndex = 1;
            Model.ShouldResetPageIndex = false;
        }

        public IQueryable<T> GenericPaging<T>(IQueryable<T> queryableData, SortingFilterModel Model)
        {
            //always recalculate the count
            var count = queryableData.Count();
            Model.PageTotal = count / Model.PageSize + (count % Model.PageSize == 0 ? 0 : 1);


            //if submit index is 0, do take no skip 
            IQueryable<T> results = null;
            if (Model.ShouldResetPageIndex || Model.PageIndex > Model.PageTotal)
                Model.PageIndex = 1;
            //if query contains no order by, perform it on the default column
            if (!Model.HasBeenSorted)
                queryableData = SortingService.SortQueryDefaultColumn(queryableData, Model);
            if (Model.PageIndex == 1)
                results = queryableData.Take(Model.PageSize);
            else
            {

                results = queryableData.Skip((Model.PageIndex - 1) * Model.PageSize).Take(Model.PageSize);
            }
            Model.ShouldResetPageIndex = false;
            ReCaculateSinglePageDatas(Model);
            return results;
        }

        public void ReCaculateSinglePageDatas(SortingFilterModel Model)
        {
            Model.SinglePageDatas.Clear();
            var SinglePageDatasTmp = new List<SinglePageData>();
            //fake list, LeftRightBuffer times 4
            int total = Model.LeftRightBuffer * 4 + 1;
            //real list, LeftRightBuffer times 2
            var total1 = Model.LeftRightBuffer * 2 + 1;
            var startindex = Model.PageIndex - Model.LeftRightBuffer * 2;
            var availableLeft = 0;
            var availableRight = 0;


            for (var i = startindex; i < total + startindex; i++)
            {

                //if the 2 pages left, 2 pages right are out side of the index, dont add it
                if (i > 0 && i <= Model.PageTotal)
                {
                    if (i < Model.PageIndex)
                        availableLeft++;
                    else if (i > Model.PageIndex)
                        availableRight++;
                    var tmp = new SinglePageData { PageLabel = i.ToString(), PageId = "singlepagedata" + i.ToString(), PageCssClass = "spd" };
                    SinglePageDatasTmp.Add(tmp);


                    if (i == Model.PageIndex)
                    {
                        tmp.IsCurrentPageSelected = true;
                        tmp.PageCssClass += " spdcurrent";
                    }
                }
            }

            //4 conditions
            if (availableLeft <= Model.LeftRightBuffer && availableRight <= Model.LeftRightBuffer)
            {
                Model.SinglePageDatas.AddRange(SinglePageDatasTmp);
            }
            else if (availableLeft > Model.LeftRightBuffer && availableRight > Model.LeftRightBuffer)
            {
                var toBeAddedElements = SinglePageDatasTmp.Skip(availableLeft - Model.LeftRightBuffer).Take(total1);
                Model.SinglePageDatas.AddRange(toBeAddedElements);
            }
            else if (availableLeft <= Model.LeftRightBuffer)
            {
                Model.SinglePageDatas.AddRange(SinglePageDatasTmp.Take(total1));
            }
            else if (availableRight <= Model.LeftRightBuffer)
            {
                SinglePageDatasTmp.Reverse();
                var toBeAddedElements = SinglePageDatasTmp.Take(total1).Reverse();
                Model.SinglePageDatas.AddRange(toBeAddedElements);
            }
        }
    }
}
