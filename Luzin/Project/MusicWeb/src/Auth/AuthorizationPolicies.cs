namespace MusicWeb.src.Auth;

public static class AuthorizationPolicies
{
    public const string AdminRole = "Admin";
    public const string ManagerRole = "Manager";
    public const string UserRole = "User";

    public const string RequireAdmin = "RequireAdmin";
    public const string RequireManagerOrAdmin = "RequireManagerOrAdmin";
    public const string RequireAnyRole = "RequireAnyRole";

    public const string CanManageContent = "CanManageContent";
    public const string CanDeleteContent = "CanDeleteContent";
    public const string CanManageUsers = "CanManageUsers";
}