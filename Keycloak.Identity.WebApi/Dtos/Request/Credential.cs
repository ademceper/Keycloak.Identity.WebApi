public class Credential
{
	public string type { get; set; } = "password"; // Sabit "password"
	public bool temporary { get; set; } = false; // Sabit false
	public string value { get; set; }
}