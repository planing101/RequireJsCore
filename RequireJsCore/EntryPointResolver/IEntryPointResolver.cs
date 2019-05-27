using Microsoft.AspNetCore.Mvc.Rendering;

namespace RequireJsCore.EntryPointResolver
{
    public interface IEntryPointResolver
    {
        string Resolve(ViewContext viewContext, RequireRendererConfiguration config);
    }
}
