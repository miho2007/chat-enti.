namespace chatapp.Entities
{
    public class User
    {
        public int Id { get; set; } // Primary key
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // For role management
    }
}
