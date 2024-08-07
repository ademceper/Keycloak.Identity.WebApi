using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class RoleService : IRoleService
{
	private readonly HttpClient _httpClient;
	private readonly KeycloakConfig _config;

	public RoleService(HttpClient httpClient, KeycloakConfig config)
	{
		_httpClient = httpClient;
		_config = config;
	}

	public async Task<IEnumerable<RoleRepresentation>> GetRolesAsync()
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Get, $"{_config.HostName}/admin/realms/{_config.Realm}/roles");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync();
		var roles = JsonSerializer.Deserialize<IEnumerable<RoleRepresentation>>(responseContent);
		return roles;
	}

	public async Task<RoleRepresentation> GetRoleByNameAsync(string roleName)
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Get, $"{_config.HostName}/admin/realms/{_config.Realm}/roles/{Uri.EscapeDataString(roleName)}");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync();
		var role = JsonSerializer.Deserialize<RoleRepresentation>(responseContent);
		return role;
	}

	public async Task CreateRoleAsync(RoleRequest roleRequest)
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.HostName}/admin/realms/{_config.Realm}/roles");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
		request.Content = new StringContent(JsonSerializer.Serialize(roleRequest), Encoding.UTF8, "application/json");

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteRoleAsync(string roleName)
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Delete, $"{_config.HostName}/admin/realms/{_config.Realm}/roles/{Uri.EscapeDataString(roleName)}");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();
	}

	public async Task AssignRolesToUserAsync(string userId, IEnumerable<RoleMappingRequest> roles)
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.HostName}/admin/realms/{_config.Realm}/users/{userId}/role-mappings/realm");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
		request.Content = new StringContent(JsonSerializer.Serialize(roles), Encoding.UTF8, "application/json");

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();
	}

	public async Task UnassignRolesFromUserAsync(string userId, IEnumerable<RoleMappingRequest> roles)
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Delete, $"{_config.HostName}/admin/realms/{_config.Realm}/users/{userId}/role-mappings/realm");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
		request.Content = new StringContent(JsonSerializer.Serialize(roles), Encoding.UTF8, "application/json");

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();
	}

	public async Task<IEnumerable<RoleRepresentation>> GetUserRolesAsync(string userId)
	{
		var adminToken = await GetAdminTokenAsync();
		var request = new HttpRequestMessage(HttpMethod.Get, $"{_config.HostName}/admin/realms/{_config.Realm}/users/{userId}/role-mappings/realm");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

		var response = await _httpClient.SendAsync(request);
		response.EnsureSuccessStatusCode();

		var responseContent = await response.Content.ReadAsStringAsync();
		var roles = JsonSerializer.Deserialize<IEnumerable<RoleRepresentation>>(responseContent);
		return roles;
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
