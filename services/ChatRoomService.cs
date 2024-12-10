using chatapp.Data;
using chatapp.Entities;
using Microsoft.EntityFrameworkCore;

namespace chatapp.Services
{
    public class ChatRoomService
    {
        private readonly AppDbContext _dbContext;

        public ChatRoomService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new chatroom with the given name.
        /// </summary>
        public async Task<ChatRoom> CreateChatRoomAsync(string name)
        {
            var newChatRoom = new ChatRoom
            {
                Name = name,
                Messages = new List<Message>() // Initialize the navigation property
            };

            await _dbContext.ChatRooms.AddAsync(newChatRoom);
            await _dbContext.SaveChangesAsync();

            return newChatRoom;
        }

        /// <summary>
        /// Fetches all available chatrooms.
        /// </summary>
        public async Task<List<ChatRoom>> GetAllChatRoomsAsync()
        {
            return await _dbContext.ChatRooms
                .Include(c => c.Messages)
                .ToListAsync();
        }

        /// <summary>
        /// Fetch a chatroom by its unique ID.
        /// </summary>
        public async Task<ChatRoom?> GetChatRoomByIdAsync(int id)
        {
            return await _dbContext.ChatRooms
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Deletes a chatroom by ID.
        /// </summary>
        public async Task<bool> DeleteChatRoomAsync(int chatRoomId)
        {
            var chatRoom = await _dbContext.ChatRooms.FindAsync(chatRoomId);
            if (chatRoom == null) return false;

            _dbContext.ChatRooms.Remove(chatRoom);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
