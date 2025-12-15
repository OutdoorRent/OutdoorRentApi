using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class OnlineMeetingHub : Hub
{
    private readonly ILogger<OnlineMeetingHub> _logger;

    public OnlineMeetingHub(ILogger<OnlineMeetingHub> logger)
    {
        _logger = logger;
    }

    private static readonly Dictionary<string, (string RoomId, string UserName)> _connections 
        = new Dictionary<string, (string, string)>();
    
    public async Task SendSignal(string roomId, string type, string data)
    {
        _logger.LogInformation("{sender} sending Signal {type}", Context.ConnectionId, type);

        string? targetConnectionId = null;
        try
        {
            using var doc = JsonDocument.Parse(data);
            if (doc.RootElement.TryGetProperty("to", out var toProp))
            {
                targetConnectionId = toProp.GetString();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse 'to' from data");
        }

        var signal = new
        {
            Type = type,
            Data = data,
            SenderId = Context.ConnectionId
        };

        if (!string.IsNullOrEmpty(targetConnectionId))
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveSignal", signal);
        }
        else
        {
            await Clients.GroupExcept(roomId, Context.ConnectionId).SendAsync("ReceiveSignal", signal);
        }
    }

    // join a room
    public async Task JoinRoom(string roomId, string userName)
    {
        _connections[Context.ConnectionId] = (roomId, userName);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        // all participants
        var participants = _connections
            .Where(pair => pair.Value.RoomId == roomId)
            .Select(kvp => new 
            {
                ConnectionId = kvp.Key,
                UserName = kvp.Value.UserName
            })
            .ToList();
        var oldParticipants = participants.Where(i =>  i.ConnectionId != Context.ConnectionId).ToList();
        // broadcast the list
        await Clients.Group(roomId).SendAsync("ParticipantsUpdated", participants);
        await Clients.GroupExcept(roomId, new[] { Context.ConnectionId })
            .SendAsync("NewParticipant", new{ ConnectionId = Context.ConnectionId, UserName = userName });
        
    }

    // user leave room
    public async Task LeaveRoom(string roomId)
    {
        _logger.LogInformation("{user} Leaving room {roomId}", Context.ConnectionId, roomId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        if (_connections.TryGetValue(Context.ConnectionId, out var info))
        {
            _connections.Remove(Context.ConnectionId);
            var participants = _connections
                .Where(pair => pair.Value.RoomId == roomId)
                .Select(kvp => new 
                {
                    ConnectionId = kvp.Key,
                    UserName = kvp.Value.UserName
                })
                .ToList();

            // broadcast the list
            await Clients.Group(roomId).SendAsync("ParticipantsUpdated", participants);
        }
    }
}