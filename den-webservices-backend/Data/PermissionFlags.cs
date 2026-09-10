namespace DenWebServices.Backend.Data;

[Flags]
public enum PermissionFlags : uint
{
    None = 0,
    Permissions = 1 << 0,
    ManageAccountStatus = 1 << 1,
    ManageAccountInfo = 1 << 2,
    ManageAccountRole = 1 << 3,
    All = 1u << 31
}