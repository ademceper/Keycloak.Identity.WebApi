using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class RoleController : ControllerBase
{
	private readonly IRoleService _roleService;

	public RoleController(IRoleService roleService)
	{
		_roleService = roleService;
	}

	[HttpGet("get-roles")]
	public async Task<IActionResult> GetRoles()
	{
		var roles = await _roleService.GetRolesAsync();
		return Ok(roles);
	}

	[HttpGet("get-role/{roleName}")]
	public async Task<IActionResult> GetRoleByName(string roleName)
	{
		var role = await _roleService.GetRoleByNameAsync(roleName);
		return Ok(role);
	}

	[HttpPost("create-role")]
	public async Task<IActionResult> CreateRole([FromBody] RoleRequest roleRequest)
	{
		await _roleService.CreateRoleAsync(roleRequest);
		return Ok(new { message = "Role created successfully" });
	}

	[HttpDelete("delete-role/{roleName}")]
	public async Task<IActionResult> DeleteRole(string roleName)
	{
		await _roleService.DeleteRoleAsync(roleName);
		return Ok(new { message = "Role deleted successfully" });
	}

	[HttpPost("assign-roles/{userId}")]
	public async Task<IActionResult> AssignRolesToUser(string userId, [FromBody] IEnumerable<RoleMappingRequest> roles)
	{
		await _roleService.AssignRolesToUserAsync(userId, roles);
		return Ok(new { message = "Roles assigned successfully" });
	}

	[HttpDelete("unassign-roles/{userId}")]
	public async Task<IActionResult> UnassignRolesFromUser(string userId, [FromBody] IEnumerable<RoleMappingRequest> roles)
	{
		await _roleService.UnassignRolesFromUserAsync(userId, roles);
		return Ok(new { message = "Roles unassigned successfully" });
	}

	[HttpGet("get-user-roles/{userId}")]
	public async Task<IActionResult> GetUserRoles(string userId)
	{
		var roles = await _roleService.GetUserRolesAsync(userId);
		return Ok(roles);
	}
}

