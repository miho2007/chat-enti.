using chatapp.Entities; // Ensure this namespace is correct

namespace chatapp.Entities
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property for related messages
        public ICollection<Message> Messages { get; set; }
    }
}
