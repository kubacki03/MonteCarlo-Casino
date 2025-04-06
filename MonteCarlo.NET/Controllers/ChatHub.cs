using Microsoft.AspNetCore.SignalR;


public class ChatHub : Hub
{
    private static readonly Dictionary<string, HashSet<string>> Rooms = new();

    // Metoda do dołączania do pokoju
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

    // Metoda do opuszczania pokoju
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

    // Wysyłanie wiadomości do określonego pokoju
    public async Task SendMessageToRoom(string roomName, string user, string message)
    {
        await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
    }

    // Metoda do uzyskania listy wszystkich dostępnych pokoi
    public Task<List<string>> GetAvailableRooms()
    {
        var roomNames = Rooms.Keys.ToList();
        return Task.FromResult(roomNames);
    }
}
