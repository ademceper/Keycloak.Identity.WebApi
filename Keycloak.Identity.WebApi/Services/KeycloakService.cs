
using System.Text.Json;


public class KeycloakService : IKeycloakService
{
	private readonly HttpClient _httpClient;
	private readonly KeycloakConfig _config;

	public KeycloakService(HttpClient httpClient, KeycloakConfig config)
	{
		_httpClient = httpClient;
		_config = config;
	}

	public async Task<string> GetTokenAsync()
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