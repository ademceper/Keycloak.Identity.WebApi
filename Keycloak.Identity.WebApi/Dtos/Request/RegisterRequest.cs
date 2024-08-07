public class RegisterRequest
{
	public string username { get; set; }
	public string firstName { get; set; }
	public string lastName { get; set; }
	public string email { get; set; }
	public bool enabled { get; set; } = true; // Sabit true
	public bool emailVerified { get; set; } = true; // Sabit true
	public Credential[] credentials { get; set; } = new[] { new Credential() };
}