using Microsoft.AspNetCore.Identity.Data;
using System.Threading.Tasks;

public interface IAuthService
{
	Task<string> GetTokenAsync(string username, string password);
	Task RegisterUserAsync(RegisterRequest registerRequest);
}
