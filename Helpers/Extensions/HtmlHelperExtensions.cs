using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Reflection;
using System.Web.Mvc.Html;

namespace Helpers.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static string GetPartialViewAsStr(this Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);

                return sw.ToString();
            }
        }

        public static MvcHtmlString HiddenForEnumerable<TModel, TProperty>(this HtmlHelper<TModel> helper,
        Expression<Func<TModel, IEnumerable<TProperty>>> expression)
        {
            var sb = new StringBuilder();

            var membername = expression.GetMemberName();
            var model = helper.ViewData.Model;
            var list = expression.Compile()(model);

            for (var i = 0; i < list.Count(); i++)
            {
                sb.Append(helper.Hidden(string.Format("{0}[{1}]", membername, i), list.ElementAt(i)));
            }

            return new MvcHtmlString(sb.ToString());
        }

        public static string GetMemberName<TModel, T>(this Expression<Func<TModel, T>> input)
        {
            if (input == null)
                return null;

            if (input.Body.NodeType != ExpressionType.MemberAccess)
                return null;

            var memberExp = input.Body as MemberExpression;
            return memberExp != null ? memberExp.Member.Name : null;
        }
    }


}
