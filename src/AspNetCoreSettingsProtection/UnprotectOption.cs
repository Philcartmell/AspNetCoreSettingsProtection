namespace AspNetCoreSettingsProtection
{
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal class UnprotectOption<T> : IPostConfigureOptions<T>
        where T : class
    {
        private readonly IDataProtectionProvider dataProtectorProvider;
        private readonly IConfiguration configuration;
        private readonly Dictionary<string, IDataProtector> dataProtectors;
        private readonly string sectionName;

        public UnprotectOption(IDataProtectionProvider dataProtectorProvider, IConfiguration configuration, string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentNullException(nameof(sectionName), "Option must have a sectionName.");
            }

            this.dataProtectorProvider = dataProtectorProvider;
            this.configuration = configuration;
            this.sectionName = sectionName;
            this.dataProtectors = new Dictionary<string, IDataProtector>();
        }

        public virtual void PostConfigure(string name, T options)
        {
            this.configuration.Bind(this.sectionName, options);

            IEnumerable<PropertyInfo> props = typeof(T)
                    .GetProperties()
                    .Where(prop =>
                        Attribute.IsDefined(prop, typeof(IsProtectedAttribute)));

            foreach (var property in props)
            {
                IsProtectedAttribute isProtectedAttribute = property.GetCustomAttributes(false).First() as IsProtectedAttribute;
                var dataProtector = this.GetProtectorForPurpose(isProtectedAttribute.ProtectorPurpose);

                if (property.PropertyType.Equals(typeof(string)))
                {
                    var protectedValue = (string)property.GetValue(options);
                    if (!String.IsNullOrEmpty(protectedValue))
                    {
                        var unprotectedValue = dataProtector.Unprotect(protectedValue);
                        property.SetValue(options, unprotectedValue);
                    }
                }
            }
        }

        internal IDataProtector GetProtectorForPurpose(string protectorPurpose)
        {
            if (!this.dataProtectors.ContainsKey(protectorPurpose))
            {
                this.dataProtectors.Add(protectorPurpose, this.dataProtectorProvider.CreateProtector(protectorPurpose));
            }

            return dataProtectors[protectorPurpose];
        }
    }
}
