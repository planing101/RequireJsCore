using Microsoft.AspNetCore.Mvc.Rendering;
using RequireJsCore.Helpers;
using System;
using System.IO;
using System.Linq;

namespace RequireJsCore.EntryPointResolver
{
    public class DefaultEntryPointResolver : IEntryPointResolver
    {
        private const string DefaultEntryPointRoot = "~/Scripts/";
        private const string DefaultArea = "Common";

        public virtual string Resolve(ViewContext viewContext, RequireRendererConfiguration config)
        {
            var routingInfo = viewContext.GetRoutingInfo();
            var rootUrl = string.Empty;
            var withBaseUrl = true;
            //var server = viewContext.HttpContext.Server;

            if (String.IsNullOrWhiteSpace(config.EntryPointRoot))
            {
                config.EntryPointRoot = config.BaseUrl;            
            }

            //var resolvedBaseUrl = UrlHelper.GenerateContentUrl(baseUrl, viewContext.HttpContext);
            //var resolvedEntryPointRoot = UrlHelper.GenerateContentUrl(entryPointRoot, viewContext.HttpContext);

            var resolvedBaseUrl = config.GenerateContentUrl(config.BaseUrl);
            var resolvedEntryPointRoot = config.GenerateContentUrl(config.EntryPointRoot);

            if (resolvedEntryPointRoot != resolvedBaseUrl)
            {
                // entryPointRoot is different from default.
                if ((config.EntryPointRoot.StartsWith("~") || config.EntryPointRoot.StartsWith("/")))
                {
                    // entryPointRoot is defined as root relative, do not use with baseUrl
                    withBaseUrl = false;
                    rootUrl = resolvedEntryPointRoot;
                }
                else
                {
                    // entryPointRoot is defined relative to baseUrl; prepend baseUrl
                    resolvedEntryPointRoot = resolvedBaseUrl + config.EntryPointRoot;
                }                
            }

            var entryPointTemplates = new[]
            {
                "Controllers\\{0}\\" + routingInfo.Controller + "\\" + routingInfo.Action,
                "Controllers\\{0}\\" + routingInfo.Controller + "\\" + routingInfo.Controller + "-" + routingInfo.Action
            };

            var areas = new[]
            {
                routingInfo.Area,
                DefaultArea
            };

            foreach (var entryPointTmpl in entryPointTemplates)
            {
                foreach (var area in areas)
                {
		            var entryPoint = area == "Root" 
                        ? entryPointTmpl.Replace("{0}\\", "").ToModuleName() 
                        : string.Format(entryPointTmpl, area).ToModuleName();

                    //var filePath = server.MapPath(entryPointRoot + entryPoint + ".js");
                    var filePath = viewContext.MapPath(config.EntryPointRoot + entryPoint + ".js");

                    if (File.Exists(filePath))
		            {
		                var computedEntry = GetEntryPoint(viewContext, filePath, config.BaseUrl);
		                return withBaseUrl ? computedEntry : rootUrl + computedEntry;
                    }
                }
            }


            return null;
        }

        private static string GetEntryPoint(ViewContext viewContext, string filePath, string root)
        {
            var fileName = PathHelpers.GetExactFilePath(filePath);
            var folder = viewContext.MapPath(root);
            return PathHelpers.GetRequireRelativePath(folder, fileName);
        }
    }
}
