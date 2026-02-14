using Microsoft.Extensions.Configuration;
using SyncUp.ClickUpApi.Auth;
using SyncUp.ClickUpApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SyncUp.ClickUpApi.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the ClickUp API client and all related services with the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="ClickUpOptions"/>.</param>
    /// <returns>The <see cref="IHttpClientBuilder"/> for further configuration.</returns>
    public static IHttpClientBuilder AddClickUpClient(
        this IServiceCollection services,
        Action<ClickUpOptions> configure)
    {
        services.Configure(configure);

        services.AddTransient<ClickUpAuthHandler>();

        var builder = services
            .AddHttpClient<IClickUpClient, ClickUpClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptionsMonitor<ClickUpOptions>>().CurrentValue;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<ClickUpAuthHandler>();

        // Also register the individual interfaces pointing to the same instance
        services.AddTransient<IClickUpAuthService>(sp => sp.GetRequiredService<IClickUpClient>());
        services.AddTransient<IClickUpWorkspaceService>(sp => sp.GetRequiredService<IClickUpClient>());
        services.AddTransient<IClickUpSpaceService>(sp => sp.GetRequiredService<IClickUpClient>());
        services.AddTransient<IClickUpFolderService>(sp => sp.GetRequiredService<IClickUpClient>());
        services.AddTransient<IClickUpListService>(sp => sp.GetRequiredService<IClickUpClient>());
        services.AddTransient<IClickUpTaskService>(sp => sp.GetRequiredService<IClickUpClient>());

        return builder;
    }

    /// <summary>
    /// Registers the ClickUp API client using configuration from an <see cref="IConfiguration"/> section.
    /// </summary>
    public static IHttpClientBuilder AddClickUpClient(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        return services.AddClickUpClient(options =>
            configuration.GetSection(ClickUpOptions.SectionName).Bind(options));
    }
}
