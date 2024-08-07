using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRoleService
{
	Task<IEnumerable<RoleRepresentation>> GetRolesAsync();
	Task<RoleRepresentation> GetRoleByNameAsync(string roleName);
	Task CreateRoleAsync(RoleRequest roleRequest);
	Task DeleteRoleAsync(string roleName);
	Task AssignRolesToUserAsync(string userId, IEnumerable<RoleMappingRequest> roles);
	Task UnassignRolesFromUserAsync(string userId, IEnumerable<RoleMappingRequest> roles);
	Task<IEnumerable<RoleRepresentation>> GetUserRolesAsync(string userId);
}
