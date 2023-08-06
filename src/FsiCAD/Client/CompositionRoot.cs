using FsiCAD.Components;
using FsiCAD.Components.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FsiCAD.Client; 

public static class CompositionRoot {
    public static WebAssemblyHostBuilder Compose(WebAssemblyHostBuilder builder) {
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddHttpClient("FsiCAD.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        // Supply HttpClient instances that include access tokens when making requests to the server project
        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("FsiCAD.ServerAPI"));

        builder.Services.AddMsalAuthentication(options => {
            builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
            options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration.GetSection("ServerApi")["Scopes"]);
        });

        builder.Services
            .AddComponents()
            .AddMessageDispatching()
            .AddStoreFactories()
            .AddIndexedDb("FsiCAD");

        return builder;
    }
    
    /// <summary>
    /// Scans the application for implementations of <see cref="IDispatchable"/> and registers a message dispatcher for them.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    static IServiceCollection AddMessageDispatching(this IServiceCollection services) {
        var asss = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.FullName).ToArray();
        var messageTypes = (
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            from iface in type.GetInterfaces()
            where iface == typeof(IDispatchable)
            select type).ToArray();

        foreach (var messageType in messageTypes) {
            services.TryAddMessageDispatcher(messageType);
        }

        return services;
    }

    /// <summary>
    /// Registers an <see cref="MessageDispatcher{TMessage}"/> for the given <paramref name="messageType"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the registrations to.</param>
    /// <param name="messageType">The type of the dispatched messages.</param>
    /// <returns>A <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection TryAddMessageDispatcher(this IServiceCollection services, Type messageType) {
        var messageDispatcherType = typeof(MessageDispatcher<>).MakeGenericType(messageType);
        services.TryAddSingleton(messageDispatcherType);
        services.TryAddSingleton(typeof(IObservable<>).MakeGenericType(messageType), sp => sp.GetRequiredService(messageDispatcherType));

        return services;
    }

    /// <summary>
    /// Scans the application for implementations of <see cref="IStoreFactory{TModel}"/> and registers them in the container.
    /// </summary>
    static IServiceCollection AddStoreFactories(this IServiceCollection services) {
        var factoryTypes =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            from iface in type.GetInterfaces()
            where iface.IsGenericType && typeof(IStoreFactory<>).IsAssignableFrom(iface.GetGenericTypeDefinition())
            select (StoreType: type, StateType: iface.GetGenericArguments()[0]);

        foreach (var (storeType, stateType) in factoryTypes) {
            services.TryAddTransient(typeof(IStoreFactory<>).MakeGenericType(stateType), storeType);
        }

        return services;
    }
}