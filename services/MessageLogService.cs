using chatapp.Data;
using chatapp.Entities;
using Microsoft.EntityFrameworkCore;

namespace chatapp.Services
{
    public class MessageLogService
    {
        private readonly AppDbContext _dbContext;

        public MessageLogService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Saves a message to the database for logging purposes.
        /// </summary>
        public async Task SaveMessageAsync(int senderId, string content, int chatRoomId)
        {
            var message = new Message
            {
                SenderId = senderId,
                Content = content,
                ChatRoomId = chatRoomId,
                SentAt = DateTime.UtcNow
            };

            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Fetch messages for a specific chatroom by its ID.
        /// </summary>
        public async Task<List<Message>> GetMessagesByChatRoomAsync(int chatRoomId)
        {
            return await _dbContext.Messages
                .Where(m => m.ChatRoomId == chatRoomId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        /// <summary>
        /// Fetch all messages for logging (admin access).
        /// </summary>
        public async Task<List<Message>> GetAllMessageLogsAsync()
        {
            return await _dbContext.Messages
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
