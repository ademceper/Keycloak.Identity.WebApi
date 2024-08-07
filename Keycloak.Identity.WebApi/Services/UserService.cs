using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class UserService : IUserService
{
	private readonly HttpClient _httpClient;
	private readonly KeycloakConfig _config;

	public UserService(HttpClient httpClient, KeycloakConfig config)
	{
		_httpClient = httpClient;
		_config = config;
	}

	public async Task<IEnumerable<UserRepresentation>> GetUsersAsync()
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Get, $"{_config.HostName}/admin/realms/{_config.Realm}/users");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync();
		var users = JsonSerializer.Deserialize<IEnumerable<UserRepresentation>>(responseContent);
		return users;
	}

	public async Task<UserRepresentation> GetUserByIdAsync(string userId)
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Get, $"{_config.HostName}/admin/realms/{_config.Realm}/users/{userId}");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync();
		var user = JsonSerializer.Deserialize<UserRepresentation>(responseContent);
		return user;
	}

	public async Task<IEnumerable<UserRepresentation>> GetUsersByParamsAsync(string username = null, string firstName = null, string lastName = null, string email = null)
	{
		var adminToken = await GetAdminTokenAsync();
		var query = new Dictionary<string, string>
		{
			["username"] = username,
			["firstName"] = firstName,
			["lastName"] = lastName,
			["email"] = email
		};

		var queryString = string.Join("&", query
			.Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
			.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));

		var requestUrl = string.IsNullOrWhiteSpace(queryString)
			? $"{_config.HostName}/admin/realms/{_config.Realm}/users"
			: $"{_config.HostName}/admin/realms/{_config.Realm}/users?{queryString}";

		var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync();
		var users = JsonSerializer.Deserialize<IEnumerable<UserRepresentation>>(responseContent);
		return users;
	}

	public async Task DeleteUserAsync(string userId)
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Delete, $"{_config.HostName}/admin/realms/{_config.Realm}/users/{userId}");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();
	}

	public async Task UpdateUserAsync(string userId, UpdateUserRequest updateUserRequest)
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Put, $"{_config.HostName}/admin/realms/{_config.Realm}/users/{userId}");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
		request.Content = new StringContent(JsonSerializer.Serialize(updateUserRequest), Encoding.UTF8, "application/json");

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();
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
