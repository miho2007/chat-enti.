using Microsoft.EntityFrameworkCore;
using chatapp.Data;
using chatapp.Entities;
using System.Security.Cryptography;
using System.Text;
using chatapp.Data;
using chatapp.Entities;

namespace chatapp.Services
{
    public class AuthenticationService
    {
        private readonly AppDbContext _dbContext;

        public AuthenticationService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Handle user registration with role assignment and password hashing
        public async Task<User> RegisterUserAsync(string username, string password, string role = "User")
        {
            // Check if the user already exists
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (existingUser != null)
            {
                throw new Exception("User already exists");
            }

            // Hash the password
            var hashedPassword = HashPassword(password);

            // Create new user
            var newUser = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                Role = role
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();
            return newUser;
        }

        // Handle user login by verifying the provided password
        public async Task<User?> LoginUserAsync(string username, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            // Verify the password
            if (VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }

            throw new Exception("Invalid password");
        }

        // Verify if a user is an admin
        public async Task<bool> IsUserAdminAsync(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            return user?.Role == "Admin";
        }

        // Hash a password securely using SHA256 (you can replace this with bcrypt or other hashing algorithms for production)
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Verify a password against a hashed password
        private bool VerifyPassword(string password, string passwordHash)
        {
            var hashedInput = HashPassword(password);
            return passwordHash == hashedInput;
        }
    }
}
