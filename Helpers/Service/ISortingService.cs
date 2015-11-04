using Helpers.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Service
{
    public interface ISortingService
    {
        List<T> GenericSort<T>(IQueryable<T> query, SortingModel Model);
        IQueryable<T> GenericSortQuery<T>(IQueryable<T> queryableData, SortingModel Model);
        IQueryable<T> SortQueryDefaultColumn<T>(IQueryable<T> queryableData, SortingModel Model, string methodName = "");
    }
}
