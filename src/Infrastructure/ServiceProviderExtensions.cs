namespace AspNetCoreSettingsProtection.Infrastructure
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// Service provider extensions
    /// </summary>
    internal static class ServiceProviderExtensions
    {
        public static T ResolveWith<T>(this IServiceProvider provider, params object[] parameters) where T : class =>
            ActivatorUtilities.CreateInstance<T>(provider, parameters);
    }
}
