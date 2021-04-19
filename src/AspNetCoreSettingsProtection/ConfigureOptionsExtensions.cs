namespace AspNetCoreSettingsProtection
{
    using AspNetCoreSettingsProtection.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using System;

    public static class ConfigureOptionsExtensions
    {
        public static IServiceCollection AddEncryptedOption<T>(this IServiceCollection services, string sectionName)
            where T : class
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentNullException(nameof(sectionName), "Option must have a sectionName.");
            }

            return services.AddSingleton<IPostConfigureOptions<T>>(p => p.ResolveWith<UnprotectOption<T>>(sectionName));
        }
    }
}
