namespace AspNetCoreSettingsProtection
{
    using System;

    /// <summary>
    /// Marks a property as being protected
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IsProtectedAttribute : Attribute
    {
        public const string DefaultProtectorPurpose = "DEFAULT";

        public IsProtectedAttribute()
            : this(DefaultProtectorPurpose)
        {
        }

        public IsProtectedAttribute(string protectorPurpose)
        {
            ProtectorPurpose = protectorPurpose;
        }

        public readonly string ProtectorPurpose;
    }
}
