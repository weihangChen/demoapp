using Helpers.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Service
{
    public interface IFilterService
    {
        IQueryable<T> AppendQueryWithContains<T>(string MemberName, string TargetValue, IQueryable<T> queryableData, SortingFilterModel Model);

        IQueryable<T> GenericPaging<T>(IQueryable<T> queryableData, SortingFilterModel Model);

        void ReCaculateSinglePageDatas(SortingFilterModel Model);
    }
}
