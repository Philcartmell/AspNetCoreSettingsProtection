namespace AspNetCoreSettingsProtection
{
    using System;

    /// <summary>
    /// Marks a property as being protected
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IsProtectedAttribute : Attribute
    {
        /// <summary>
        /// Mark property as protected by specific protector purpose.
        /// </summary>
        /// <param name="protectorPurpose">Decrypt using specified protector purpose.</param>
        public IsProtectedAttribute(string protectorPurpose)
        {
            ProtectorPurpose = protectorPurpose;
        }

        public readonly string ProtectorPurpose;
    }
}
