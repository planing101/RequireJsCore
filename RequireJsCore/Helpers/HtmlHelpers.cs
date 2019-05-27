// RequireJS.NET
// Copyright VeriTech.io
// http://veritech.io
// Dual licensed under the MIT and GPL licenses:
// http://www.opensource.org/licenses/mit-license.php
// http://www.gnu.org/licenses/gpl.html

using Microsoft.AspNetCore.Mvc.Rendering;
using RequireJsCore.Models;

namespace RequireJsCore.Helpers
{
    internal static class HtmlHelpers
    {
        public static RoutingInfo GetRoutingInfo(this ViewContext viewContext)
        {
            var area = viewContext.RouteData.DataTokens["area"] != null
                ? viewContext.RouteData.DataTokens["area"].ToString()
                : "Root";

            //var controller = viewContext.Controller.ValueProvider.GetValue("controller").RawValue as string;
            //var action = viewContext.Controller.ValueProvider.GetValue("action").RawValue as string;
            var controller = viewContext.RouteData.Values["controller"] as string;
            var action = viewContext.RouteData.Values["action"] as string;

            return new RoutingInfo
            {
                Area = area,
                Controller = controller,
                Action = action
            };
        }
    }
}
