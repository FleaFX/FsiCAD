using Microsoft.JSInterop;

namespace FsiCAD.Components.Extensions; 

public static class ExtensionsForIJsRuntime {
    /// <summary>
    /// Asynchronously imports the module with the given name.
    /// </summary>
    /// <param name="jsRuntime">The <see cref="IJSRuntime"/> to use.</param>
    /// <param name="moduleName">The name of the module to import.</param>
    /// <param name="modulePath">Optional path to load the module from. Defaults to <c>./_content/FsiCAD.Components</c>.</param>
    /// <returns></returns>
    public static ValueTask<IJSObjectReference> ImportModuleAsync(this IJSRuntime jsRuntime, string moduleName, string modulePath = "./_content/FsiCAD.Components/scripts") =>
        jsRuntime.InvokeAsync<IJSObjectReference>("import", $"{Path.Combine(modulePath, moduleName).Replace("\\", "/")}.js");
}