public interface IKeycloakService
{
	Task<string> GetTokenAsync();
}