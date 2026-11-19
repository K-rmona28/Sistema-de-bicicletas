using ApiCycleOn.Exceptions;
using ApiCycleOn.Facades;
using ApiCycleOn.Patterns.Command;
using ApiCycleOn.Patterns.Observer;
using ApiCycleOn.Patterns.Strategy;
using ApiCycleOn.Services;

namespace ApiCycleOn.Extensions;

public static class ServiceCollectionExtensions
{
    // En Program.cs:  builder.Services.AddCycleOnServices();
    public static IServiceCollection AddCycleOnServices(this IServiceCollection services)
    {
        // Servicios base
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenService, TokenService>();

        // STRATEGY: costo y canal de notificación (sin estado => singleton)
        services.AddSingleton<ICostoStrategy, CostoProporcionalStrategy>();
        services.AddSingleton<ICostoStrategy, CostoHoraCompletaStrategy>();
        services.AddSingleton<ICostoStrategy, CostoFraccion15Strategy>();
        services.AddSingleton<CostoStrategyResolver>();
        services.AddSingleton<INotificacionStrategy, EmailNotificacionStrategy>();
        services.AddSingleton<INotificacionStrategy, WhatsAppNotificacionStrategy>();

        // OBSERVER: publicador + todos los IObservador<> del proyecto, detectados automáticamente
        services.AddScoped<IPublicadorEventos, PublicadorEventos>();
        var tiposObservador = typeof(ServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false });

        foreach (var tipo in tiposObservador)
            foreach (var contrato in tipo.GetInterfaces()
                         .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IObservador<>)))
                services.AddScoped(contrato, tipo);

        // Notificaciones en segundo plano (cola + worker)
        services.AddSingleton<ColaNotificaciones>();
        services.AddSingleton<IColaNotificaciones>(sp => sp.GetRequiredService<ColaNotificaciones>());
        services.AddHostedService<NotificacionWorker>();

        // COMMAND
        services.AddScoped<InvocadorComandos>();

        // FACADE
        services.AddScoped<AuthFacade>();
        services.AddScoped<UsuarioFacade>();

        // Manejo global de errores (Program.cs debe llamar app.UseExceptionHandler())
        services.AddExceptionHandler<ManejadorExcepciones>();
        services.AddProblemDetails();

        return services;
    }
}
