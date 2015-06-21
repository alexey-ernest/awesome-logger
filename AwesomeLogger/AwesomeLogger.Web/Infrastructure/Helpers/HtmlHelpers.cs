using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace AwesomeLogger.Web.Infrastructure.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString RequiredFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (metadata.IsRequired)
            {
                return new MvcHtmlString("required");
            }

            return new MvcHtmlString(string.Empty);
        }
    }
}