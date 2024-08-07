using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class AuthService : IAuthService
{
	private readonly HttpClient _httpClient;
	private readonly KeycloakConfig _config;

	public AuthService(HttpClient httpClient, KeycloakConfig config)
	{
		_httpClient = httpClient;
		_config = config;
	}

	public async Task<string> GetTokenAsync(string username, string password)
	{
		var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.HostName}/realms/{_config.Realm}/protocol/openid-connect/token");
		var content = new FormUrlEncodedContent(new[]
		{
			new KeyValuePair<string, string>("grant_type", "password"),
			new KeyValuePair<string, string>("client_id", _config.ClientId),
			new KeyValuePair<string, string>("client_secret", _config.ClientSecret),
			new KeyValuePair<string, string>("username", username),
			new KeyValuePair<string, string>("password", password)
		});

		request.Content = content;
		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync();
		var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
		return tokenResponse["access_token"].ToString();
	}

	public async Task RegisterUserAsync(RegisterRequest registerRequest)
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.HostName}/admin/realms/{_config.Realm}/users");

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
		request.Content = new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json");

		var response = await _httpClient.SendAsync(request);

		if (!response.IsSuccessStatusCode)
		{
			var responseBody = await response.Content.ReadAsStringAsync();
			throw new HttpRequestException($"Error registering user: {responseBody}");
		}
	}

	private async Task<string> GetAdminTokenAsync()
	{
		var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.HostName}/realms/{_config.Realm}/protocol/openid-connect/token");
		var content = new FormUrlEncodedContent(new[]
		{
			new KeyValuePair<string, string>("grant_type", "client_credentials"),
			new KeyValuePair<string, string>("client_id", _config.ClientId),
			new KeyValuePair<string, string>("client_secret", _config.ClientSecret)
		});

		request.Content = content;
		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync();
		var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
		return tokenResponse["access_token"].ToString();
	}
}
