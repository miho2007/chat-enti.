using chatapp.Entities; // Ensure this namespace is correct

namespace chatapp.Entities
{
    public class Message
    {
        public int Id { get; set; } // Primary Key
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow; // When the message was sent
        public int SenderId { get; set; } // Foreign key linking to the user
        public User Sender { get; set; } // Navigation property
        public int ChatRoomId { get; set; } // Foreign key linking the message to a chatroom
        public ChatRoom ChatRoom { get; set; } // Navigation property
    }
}
