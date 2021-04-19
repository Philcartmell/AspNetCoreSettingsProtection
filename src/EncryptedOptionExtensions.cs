namespace AspNetCoreSettingsProtection
{
    using AspNetCoreSettingsProtection.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using System;

    /// <summary>
    /// IServiceCollection extension methods for adding encrypted options.
    /// </summary>
    public static class EncryptedOptionExtensions
    {
        /// <summary>
        /// Adds an encrypted option for the given section name
        /// </summary>
        /// <typeparam name="T">Type to deserialise into.</typeparam>
        /// <param name="services">IServiceCollection</param>
        /// <param name="sectionName">Section name. For nested structures use : (colon) to separate levels.</param>
        /// <returns></returns>
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
