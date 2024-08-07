using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
	{
		_authService = authService;
	}

	[HttpPost("login")]
	public async Task<IActionResult> GetToken([FromBody] LoginRequest loginRequest)
	{
		var token = await _authService.GetTokenAsync(loginRequest.Username, loginRequest.Password);
		return Ok(new { access_token = token });
	}

	[HttpPost("register")]
	public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest registerRequest)
	{
		await _authService.RegisterUserAsync(registerRequest);
		return Ok(new { message = "User registered successfully" });
	}
}