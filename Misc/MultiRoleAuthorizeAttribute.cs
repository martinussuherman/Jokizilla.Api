using Microsoft.AspNetCore.Authorization;

namespace Jokizilla.Api.Misc
{
    public class MultiRoleAuthorizeAttribute : AuthorizeAttribute
    {
        public MultiRoleAuthorizeAttribute(params RoleEnum[] allowedRoles)
        {
            AllowedRoles = allowedRoles;
            Roles = string.Join(',', allowedRoles);
        }

        public RoleEnum[] AllowedRoles { get; }
    }
}
