// RequireJS.NET
// Copyright VeriTech.io
// http://veritech.io
// Dual licensed under the MIT and GPL licenses:
// http://www.opensource.org/licenses/mit-license.php
// http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using RequireJsCore.EntryPointResolver;
using RequireJsCore;

namespace RequireJsCore
{
    public enum RequireJsOptionsScope
    {
        Page,
        Global
    }

    public static class RequireJsOptions
    {
        private const string GlobalOptionsKey = "globalOptions";

        private const string PageOptionsKey = "pageOptions";

        public static readonly RequireEntryPointResolverCollection ResolverCollection = new RequireEntryPointResolverCollection();

        static RequireJsOptions()
        {
            ResolverCollection.Add(new DefaultEntryPointResolver());
        }

        public static Dictionary<string, object> GetGlobalOptions(HttpContext context)
        {
            var page = context.Items[GlobalOptionsKey] as Dictionary<string, object>;
            if (page == null)
            {
                context.Items[GlobalOptionsKey] = new Dictionary<string, object>();
            }

            return (Dictionary<string, object>)context.Items[GlobalOptionsKey];
        }

        public static Dictionary<string, object> GetPageOptions(HttpContext context)
        {
            var page = context.Items[PageOptionsKey] as Dictionary<string, object>;
            if (page == null)
            {
                context.Items[PageOptionsKey] = new Dictionary<string, object>();
            }

            return (Dictionary<string, object>)context.Items[PageOptionsKey];
        }

        public static void Add(HttpContext context, string key, object value, RequireJsOptionsScope scope = RequireJsOptionsScope.Page)
        {
            switch (scope)
            {
                case RequireJsOptionsScope.Page:
                    var pageOptions = GetPageOptions(context);
                    if (pageOptions.Keys.Contains(key))
                    {
                        pageOptions.Remove(key);
                    }

                    pageOptions.Add(key, value);
                    break;
                case RequireJsOptionsScope.Global:
                    var globalOptions = GetGlobalOptions(context);
                    if (globalOptions.Keys.Contains(key))
                    {
                        globalOptions.Remove(key);
                    }

                    globalOptions.Add(key, value);
                    break;
            }
        }


        public static void Add(HttpContext context,
            string key,
            Dictionary<string, object> value,
            RequireJsOptionsScope scope = RequireJsOptionsScope.Page,
            bool clearExisting = false)
        {
            var dictToModify = new Dictionary<string, object>();
            switch (scope)
            {
                case RequireJsOptionsScope.Page:
                    dictToModify = GetPageOptions(context);
                    break;
                case RequireJsOptionsScope.Global:
                    dictToModify = GetGlobalOptions(context);
                    break;
            }

            var existing = dictToModify.FirstOrDefault(r => r.Key == key).Value;
            if (existing != null)
            {
                if (!clearExisting && existing is Dictionary<string, object>)
                {
                    AppendItems(existing as Dictionary<string, object>, value);
                }
                else
                {
                    dictToModify.Remove(key);
                    dictToModify.Add(key, value);
                }
            }
            else
            {
                dictToModify.Add(key, value);
            }
        }

        public static object GetByKey(HttpContext httpContext, string key, RequireJsOptionsScope scope)
        {
            return scope == RequireJsOptionsScope.Page ? GetPageOptions(httpContext).FirstOrDefault(r => r.Key == key)
                                                       : GetGlobalOptions(httpContext).FirstOrDefault(r => r.Key == key);
        }

        public static void Clear(HttpContext httpContext, RequireJsOptionsScope scope)
        {
            switch (scope)
            {
                case RequireJsOptionsScope.Page:
                    GetPageOptions(httpContext).Clear();
                    break;
                case RequireJsOptionsScope.Global:
                    GetGlobalOptions(httpContext).Clear();
                    break;
            }
        }

        public static void ClearAll(HttpContext httpContext)
        {
            Clear(httpContext, RequireJsOptionsScope.Global);
            Clear(httpContext, RequireJsOptionsScope.Page);
        }
        

        private static void AppendItems(Dictionary<string, object> to, Dictionary<string, object> from)
        {
            foreach (var item in from)
            {
                var existing = to.FirstOrDefault(r => item.Key == r.Key).Value;
                if (existing != null)
                {
                    to.Remove(item.Key);
                }

                to.Add(item.Key, item.Value);
            }
        }

        private static HttpContext GetCurrentContext(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new Exception("HttpContext.Current is null. RequireJsNet needs a HttpContext in order to work.");
            }

            return httpContext;
        }
    }
}