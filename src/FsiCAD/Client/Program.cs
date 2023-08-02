using FsiCAD.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

await CompositionRoot
    .Compose(WebAssemblyHostBuilder.CreateDefault(args))
    .Build()
    .RunAsync();