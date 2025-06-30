using Microsoft.AspNetCore.SignalR;

namespace PujaService.Api.Hubs;

public class PujasHub : Hub
{
    public async Task UnirseASubasta(string subastaId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, subastaId);
        Console.WriteLine($"Usuario {Context.ConnectionId} se unió a la subasta {subastaId}");
    }
}
