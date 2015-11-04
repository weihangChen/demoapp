using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.ExpressionTree
{
    public interface IExpressionBuilder
    {
        Expression<Func<T, K>> CreatePropertyExpression<T, K>(string propertyName);

        Expression CreatePropertyExpressionNoConvert<T>(string propertyName);

        MethodCallExpression BuildMethodExpression<TSource>(IQueryable<TSource> query, string methodName, Expression Exp, Type[] typeArguments = null);

        Expression EvaluateChainedRule<T>(ChainedRule currentRule, ParameterExpression p, Expression exp = null);
    }
}
