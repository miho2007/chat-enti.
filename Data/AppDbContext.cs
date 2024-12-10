using chatapp.Entities;
using Microsoft.EntityFrameworkCore;

namespace chatapp.Data
{
    public class AppDbContext : DbContext
    {
        private const string ConnectionString = "Data Source=MIHO\\MISHO;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public AppDbContext() : base(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString).Options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Message>().HasKey(m => m.Id);
            modelBuilder.Entity<ChatRoom>().HasKey(c => c.Id);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId);
        }
    }
}
