
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
	private readonly IUserService _userService;

	public UserController(IUserService userService)
	{
		_userService = userService;
	}

	[HttpGet("get-users")]
	public async Task<IActionResult> GetUsers()
	{
		var users = await _userService.GetUsersAsync();
		return Ok(users);
	}

	[HttpGet("get-user/{userId}")]
	public async Task<IActionResult> GetUserById(string userId)
	{
		var user = await _userService.GetUserByIdAsync(userId);
		return Ok(user);
	}

	[HttpGet("search-users")]
	public async Task<IActionResult> GetUsersByParams([FromQuery] string username = null, [FromQuery] string firstName = null, [FromQuery] string lastName = null, [FromQuery] string email = null)
	{
		var users = await _userService.GetUsersByParamsAsync(username, firstName, lastName, email);
		return Ok(users);
	}

	[HttpDelete("delete-user/{userId}")]
	public async Task<IActionResult> DeleteUser(string userId)
	{
		await _userService.DeleteUserAsync(userId);
		return Ok(new { message = "User deleted successfully" });
	}

	[HttpPut("update-user/{userId}")]
	public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserRequest updateUserRequest)
	{
		await _userService.UpdateUserAsync(userId, updateUserRequest);
		return Ok(new { message = "User updated successfully" });
	}
}