public class UpdateUserRequest
{
	public string firstName { get; set; }
	public string lastName { get; set; }
	public string email { get; set; }
	public bool enabled { get; set; }
	public bool emailVerified { get; set; }
	public Credential[] credentials { get; set; }
}