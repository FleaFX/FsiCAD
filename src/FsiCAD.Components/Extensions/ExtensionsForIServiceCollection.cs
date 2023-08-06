using System.Reflection;
using FsiCAD.Components.Services;
using FsiCAD.Components.Services.IndexedDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;

namespace FsiCAD.Components.Extensions; 

public static class ExtensionsForIServiceCollection {
    /// <summary>
    /// Makes the necessary registrations to use FsiCAD.Components.
    /// </summary>
    public static IServiceCollection AddComponents(this IServiceCollection services) =>
        services
            .AddMessageDispatching();

    /// <summary>
    /// Scans the application for implementations of <see cref="IDispatchable"/> and registers a message dispatcher for them.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    static IServiceCollection AddMessageDispatching(this IServiceCollection services) {
        var messageTypes = (
            from type in Assembly.GetAssembly(typeof(ExtensionsForIServiceCollection))!.GetTypes()
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
    static IServiceCollection TryAddMessageDispatcher(this IServiceCollection services, Type messageType) {
        var messageDispatcherType = typeof(MessageDispatcher<>).MakeGenericType(messageType);
        services.TryAddSingleton(messageDispatcherType);
        services.TryAddSingleton(typeof(IObservable<>).MakeGenericType(messageType), sp => sp.GetRequiredService(messageDispatcherType));

        return services;
    }

    /// <summary>
    /// Adds the necessary registrations to work with IndexedDB.
    /// </summary>
    public static IServiceCollection AddIndexedDb(this IServiceCollection services, string databaseName, int version = 1) {
        services.TryAddSingleton(sp => new IndexedDbConnectionFactory(sp.GetRequiredService<IJSRuntime>(), databaseName, version));
        services.TryAddTransient(typeof(IRepository<>), typeof(IndexedDbRepository<>));
        return services;
    }
}