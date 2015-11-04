using Helpers.ExpressionTree;
using Helpers.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Helpers.Extensions;
using Helpers.ExpressionTree;

namespace Helpers.Service
{
    public class SortingService : ISortingService
    {
        private IExpressionBuilder ExpBuilder;
        public SortingService(IExpressionBuilder ExpBuilder)
        {
            this.ExpBuilder = ExpBuilder;
        }
        public List<T> GenericSort<T>(IQueryable<T> query, SortingModel Model)
        {
            var result = new List<T>();
            if (Model.SortingColumnNew.IsEmpty())
                return query.ToList();

            Expression<Func<T, object>> orderByExp = ExpBuilder.CreatePropertyExpression<T, object>(Model.SortingColumnNew.Trim());
            bool OrderByDescending = CalculateSortingOrder(Model);
            if (OrderByDescending)
                result = query.OrderByDescending(orderByExp).ToList();
            else
                result = query.OrderBy(orderByExp).ToList();
            Model.ShouldReOrder = false;
            return result;
        }

        protected bool CalculateSortingOrder(SortingModel Model)
        {
            bool OrderByDescending = true;
            if (Model.SortingColumnNew.IsEmpty())
                return OrderByDescending;

            if (!Model.SortingColumnCurrent.EqualIgnoreCase(Model.SortingColumnNew))
            {
                OrderByDescending = false;
                Model.SortingColumnCurrent = Model.SortingColumnNew;
                if (Model.SortingOrderCurrent == SortingOrder.Descending)
                {
                    Model.SortingOrderCurrent = SortingOrder.Ascending;
                }
                else if (Model.SortingOrderCurrent == SortingOrder.Ascending)
                {
                    Model.SortingOrderCurrent = SortingOrder.Descending;
                }
            }
            else
            {
                if (Model.SortingOrderCurrent == SortingOrder.Descending)
                {
                    OrderByDescending = false;
                    Model.SortingOrderCurrent = SortingOrder.Ascending;
                }
                else if (Model.SortingOrderCurrent == SortingOrder.Ascending)
                {
                    Model.SortingOrderCurrent = SortingOrder.Descending;
                }
            }
            return OrderByDescending;
        }
        public string GetOrderbyMethodName(bool OrderByDescending)
        {
            string methodName = "OrderByDescending";
            if (!OrderByDescending)
                methodName = "OrderBy";
            return methodName;
        }

        public IQueryable<T> GenericSortQuery<T>(IQueryable<T> queryableData, SortingModel Model)
        {
            IQueryable<T> results = null;
            if (!Model.ShouldReOrder)
            {
                string methodName = GetOrderbyMethodName(Model.SortingOrderCurrent == SortingOrder.Descending ? true : false);
                results = SortQueryDefaultColumn(queryableData, Model, methodName);
                return results;
            }


            if (Model.SortingColumnNew.IsEmpty())
            {
                results = SortQueryDefaultColumn(queryableData, Model);
            }
            else
            {
                var orderByExp = ExpBuilder.CreatePropertyExpressionNoConvert<T>(Model.SortingColumnNew.Trim());
                bool OrderByDescending = CalculateSortingOrder(Model);

                string methodName = GetOrderbyMethodName(OrderByDescending);
                var methodExp = ExpBuilder.BuildMethodExpression(queryableData, methodName, orderByExp);
                results = queryableData.EvalQueryMethodCallExpression(methodExp);
            }
            Model.HasBeenSorted = true;
            Model.ShouldReOrder = false;
            return results;
        }


        public IQueryable<T> SortQueryDefaultColumn<T>(IQueryable<T> queryableData, SortingModel Model, string methodName = "")
        {
            if (Model.SortingColumnCurrent.IsEmpty())
                throw new ArgumentException("no SortingColumnCurrent is defined in viewmodel");


            var orderByExp = ExpBuilder.CreatePropertyExpressionNoConvert<T>(Model.SortingColumnCurrent.Trim());
            if (methodName.IsEmpty())
                methodName = "OrderByDescending";
            var methodExp = ExpBuilder.BuildMethodExpression(queryableData, methodName, orderByExp);
            IQueryable<T> results = queryableData.EvalQueryMethodCallExpression(methodExp);
            Model.HasBeenSorted = true;
            return results;
        }

    }
}
