using Microsoft.AspNetCore.SignalR;


public class ChatHub : Hub
{
    private static readonly Dictionary<string, HashSet<string>> Rooms = new();

    public async Task JoinRoom(string roomName)
    {
        if (!Rooms.ContainsKey(roomName))
        {
            Rooms[roomName] = new HashSet<string>();
        }

        Rooms[roomName].Add(Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} joined the room {roomName}");
    }

    public async Task LeaveRoom(string roomName)
    {
        if (Rooms.ContainsKey(roomName) && Rooms[roomName].Contains(Context.ConnectionId))
        {
            Rooms[roomName].Remove(Context.ConnectionId);

            if (Rooms[roomName].Count == 0)
            {
                Rooms.Remove(roomName);
            }
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} left the room {roomName}");
    }

    public async Task SendMessageToRoom(string roomName, string user, string message)
    {
        await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
    }

    public Task<List<string>> GetAvailableRooms()
    {
        var roomNames = Rooms.Keys.ToList();
        return Task.FromResult(roomNames);
    }
}
