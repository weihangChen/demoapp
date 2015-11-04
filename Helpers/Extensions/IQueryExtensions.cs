using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Extensions
{
    public static class IQueryExtensions
    {
        public static IQueryable<T> EvalQueryMethodCallExpression<T>(this IQueryable<T> query, MethodCallExpression Exp)
        {
            IQueryable<T> results = query.Provider.CreateQuery<T>(Exp);
            return results;
        }



    }
}
