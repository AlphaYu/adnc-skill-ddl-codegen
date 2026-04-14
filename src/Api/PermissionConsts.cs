namespace Adnc.Skill.Test.Api;

/// <summary>
/// Defines permission codes used by the application layer.
/// </summary>
public static class PermissionConsts
{
    /// <summary>
    /// Defines permission codes for customer management.
    /// </summary>
    public static class Customer
    {
        public const string Create = "customer-create";
        public const string Update = "customer-update";
        public const string Delete = "customer-delete";
        public const string Search = "customer-search";
        public const string Get = "customer-get";
    }

    /// <summary>
    /// Defines permission codes for notice management.
    /// </summary>
    public static class Notice
    {
        public const string Create = "notice-create";
        public const string Update = "notice-update";
        public const string Delete = "notice-delete";
        public const string Search = "notice-search";
        public const string Get = "notice-get";
    }

    /// <summary>
    /// Defines permission codes for tenant management.
    /// </summary>
    public static class Tenant
    {
        public const string Create = "tenant-create";
        public const string Update = "tenant-update";
        public const string Delete = "tenant-delete";
        public const string Search = "tenant-search";
        public const string Get = "tenant-get";
    }
}
