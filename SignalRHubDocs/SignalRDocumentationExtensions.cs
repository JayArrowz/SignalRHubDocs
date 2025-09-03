using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SignalRHubDocs;

public static class SignalRDocumentationExtensions
{
    public static IServiceCollection AddSignalRDocumentation(this IServiceCollection services, Action<SignalRDocumentationOptions>? configure = null)
    {
        var options = new SignalRDocumentationOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<HubInspectorService>();
        services.AddSingleton<EmbeddedTemplateService>();

        return services;
    }

    public static IApplicationBuilder UseSignalRDocumentation(this IApplicationBuilder app, Action<SignalRDocumentationOptions>? configure = null)
    {
        var options = app.ApplicationServices.GetService<SignalRDocumentationOptions>() ?? new SignalRDocumentationOptions();
        configure?.Invoke(options);

        return app.UseMiddleware<SignalRDocumentationMiddleware>();
    }
}
