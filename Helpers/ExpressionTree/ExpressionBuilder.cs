using Helpers.ExpressionTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Helpers.ExpressionTree
{
    public class ExpressionBuilder : IExpressionBuilder
    {
        public Expression<Func<T, K>> CreatePropertyExpression<T, K>(string propertyName)
        {
            PropertyInfo property = typeof(T).GetProperty(propertyName.Trim());
            if (property == null)
                throw new ArgumentException("property name is wrong, no variable can be found via reflection");
            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            Expression orderBy = Expression.Property(param, property);
            Expression<Func<T, K>> lambda = Expression.Lambda<Func<T, K>>(Expression.Convert(orderBy, typeof(K)), param);

            return lambda;
        }

        public Expression CreatePropertyExpressionNoConvert<T>(string propertyName)
        {
            PropertyInfo property = typeof(T).GetProperty(propertyName.Trim());

            if (property == null)
                throw new ArgumentException("property name is wrong, no variable can be found via reflection");
            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            Expression orderBy = Expression.Property(param, property);
            var lambda = Expression.Lambda(orderBy, param);

            return lambda;
        }

        /// <summary>
        /// can be invoked directed to get an expression back or used in compilerule to get a method back
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r"></param>
        /// <param name="param">when param is not null, mostly invoked in compilerule, otherwise I am after an expression only</param>
        /// <returns></returns>
        public Expression BuildExpr<T>(Rule r, ParameterExpression param = null)
        {
            if (param == null)
                param = Expression.Parameter(typeof(T), ParamName.x.ToString());
            Expression left = null;

            if (r.Exp != null)
                left = r.Exp;
            else
                left = MemberExpression.Property(param, r.MemberName);

            //reflection
            var tProp = typeof(T).GetProperty(r.MemberName).PropertyType;

            ExpressionType tBinary;

            // is the operator a known .NET operator?

            if (ExpressionType.TryParse(r.Operator.ToString(), out tBinary))
            {

                var right = Expression.Constant(Convert.ChangeType(r.TargetValue, tProp));

                // use a binary operation, e.g. 'Equal' -> 'u.Age == 15'

                return Expression.MakeBinary(tBinary, left, right);

            }
            else
            {
                if (r.TargetValue == null)
                {
                    var method1 = tProp.GetMethod(r.Operator.ToString(), System.Type.EmptyTypes);
                    return Expression.Call(left, method1);
                }
                var method = tProp.GetMethod(r.Operator.ToString());

                var tParam = method.GetParameters()[0].ParameterType;

                var right = Expression.Constant(Convert.ChangeType(r.TargetValue, tParam));

                // use a method call, e.g. 'Contains' -> 'u.Tags.Contains(some_tag)'

                return Expression.Call(left, method, right);

            }

        }



        public Func<T, bool> CompileRule<T>(Rule r)
        {
            var paramUser = Expression.Parameter(typeof(T));
            Expression expr = BuildExpr<T>(r, paramUser);
            return Expression.Lambda<Func<T, bool>>(expr, paramUser).Compile();

        }

        public Expression EvaluateChainedRule<T>(ChainedRule currentRule, ParameterExpression p, Expression exp = null)
        {
            if (currentRule == null)
                return exp;

            Expression tmpExp = BuildExpr<T>(currentRule, p);
            if (currentRule.ChildRule != null)
                currentRule.ChildRule.Exp = tmpExp;
            return EvaluateChainedRule<T>(currentRule.ChildRule, p, tmpExp);

        }



        public MethodCallExpression BuildMethodExpression<TSource>(IQueryable<TSource> query, string methodName, Expression Exp, Type[] typeArguments = null)
        {
            if (typeArguments == null)
                typeArguments = Exp.Type.GetGenericArguments();

            var methodCall = Expression.Call(
                typeof(Queryable),
                methodName,
                typeArguments,
                query.Expression,
                Exp);
            return methodCall;
        }
    }
}
