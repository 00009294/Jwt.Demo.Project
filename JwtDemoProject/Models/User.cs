namespace JwtDemoProject.Models
{
    public class User
    {
        public string Username { get; set; } = String.Empty;
        public byte[] PassordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
