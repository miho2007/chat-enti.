using chatapp.Data;
using chatapp.Entities;
using chatapp.Services;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Starting Chat Application...");

// Initialize the database context
var dbContext = new AppDbContext();

// Ensure database migrations are applied
dbContext.Database.Migrate();

// Initialize services
var authService = new AuthenticationService(dbContext);
var chatRoomService = new ChatRoomService(dbContext);
var messageLogService = new MessageLogService(dbContext);

await ShowMenu(authService, chatRoomService, messageLogService);

async Task ShowMenu(AuthenticationService authService, ChatRoomService chatRoomService, MessageLogService messageLogService)
{
    while (true)
    {
        Console.WriteLine("\n--- ChatApp Menu ---");
        Console.WriteLine("1. Register");
        Console.WriteLine("2. Login");
        Console.WriteLine("3. Exit");
        Console.Write("Choose an option: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await Register(authService);
                break;
            case "2":
                var user = await Login(authService);
                if (user != null)
                {
                    await Chat(user, chatRoomService, messageLogService);
                }
                break;
            case "3":
                Console.WriteLine("Exiting ChatApp. Goodbye!");
                return;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }
}

async Task Register(AuthenticationService authService)
{
    Console.WriteLine("\n--- Register ---");
    Console.Write("Enter a username: ");
    var username = Console.ReadLine();

    Console.Write("Enter a password: ");
    var password = Console.ReadLine();

    try
    {
        var registeredUser = await authService.RegisterUserAsync(username, password);
        Console.WriteLine($"User '{registeredUser.Username}' successfully registered!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred during registration: {ex.Message}");
    }
}

async Task<User> Login(AuthenticationService authService)
{
    Console.WriteLine("\n--- Login ---");
    Console.Write("Enter your username: ");
    var username = Console.ReadLine();

    Console.Write("Enter your password: ");
    var password = Console.ReadLine();

    try
    {
        var user = await authService.LoginUserAsync(username, password);
        Console.WriteLine($"Welcome back, {user.Username}!");
        return user;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Login failed: {ex.Message}");
        return null;
    }
}

async Task Chat(User user, ChatRoomService chatRoomService, MessageLogService messageLogService)
{
    Console.WriteLine("\n--- Chat ---");

    var chatRooms = await chatRoomService.GetAllChatRoomsAsync();
    if (!chatRooms.Any())
    {
        Console.WriteLine("No chatrooms found. Creating default chatroom...");
        var defaultRoom = await chatRoomService.CreateChatRoomAsync("General");
        Console.WriteLine($"Chatroom '{defaultRoom.Name}' created.");
        chatRooms.Add(defaultRoom);
    }

    Console.WriteLine("Available ChatRooms:");
    foreach (var chatRoom in chatRooms)
    {
        Console.WriteLine($"- {chatRoom.Id}: {chatRoom.Name}");
    }

    Console.Write("Enter the ID of the chatroom you want to join: ");
    var chatRoomId = int.Parse(Console.ReadLine());

    var selectedChatRoom = chatRooms.FirstOrDefault(r => r.Id == chatRoomId);
    if (selectedChatRoom == null)
    {
        Console.WriteLine("Invalid chatroom ID. Returning to menu.");
        return;
    }

    Console.WriteLine($"Joined chatroom: {selectedChatRoom.Name}");

    while (true)
    {
        Console.Write("Enter a message (or type 'exit' to leave): ");
        var msgContent = Console.ReadLine();

        if (msgContent?.ToLower() == "exit")
        {
            Console.WriteLine("Leaving chatroom...");
            break;
        }

        try
        {
            await messageLogService.SaveMessageAsync(user.Id, msgContent, selectedChatRoom.Id);
            Console.WriteLine($"Message sent: '{msgContent}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send message: {ex.Message}");
        }
    }
}
