using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
	private readonly IKeycloakService _keycloakService;

	public HomeController(IKeycloakService keycloakService)
	{
		_keycloakService = keycloakService;
	}

	[HttpGet("get-token")]
	public async Task<IActionResult> GetToken()
	{
		var token = await _keycloakService.GetTokenAsync();
		return Ok(new { access_token = token });
	}
}
