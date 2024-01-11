using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UdemySignalR.API.Models;

namespace UdemySignalR.API.Hubs;

public class MyHub : Hub
{
    private readonly AppDbContext _context;

    public MyHub(AppDbContext context)
    {
        _context = context;
    }

    private static List<string> Names { get; set; } = new List<string>();
    private static List<string> ClientList { get; set; } = new List<string>();
    private static int ClientCount { get; set; } = 0;

    public static int TeamCount { get; set; } = 7;

    public async Task SendProduct(Product p)
    {
        await Clients.All.SendAsync("ReceiveProduct", p);
    }

    public async Task SendName(string name)
    {
        if (Names.Count >= TeamCount)
        {
            await Clients.Caller.SendAsync("Error", $"Takım en fazla {TeamCount} kişi olabilir.");
        }
        else
        {
            Names.Add(name);
            await Clients.All.SendAsync("ReceiveName", name);
        }


    }

    public async Task GetNames()
    {
        await Clients.All.SendAsync("ReceiveNames", Names);
    }


    public async Task AddToGroup(string teamName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
    }

    public async Task SendNameByGroup(string name, string teamName)
    {
        var team = _context.Teams.Where(x => x.Name == teamName).FirstOrDefault();
        if (team != null)
        {
            team.Users.Add(new User { Name = name });
        }
        else
        {
            var newTeam = new Team { Name = name };
            newTeam.Users.Add(new User { Name = name });
            await _context.AddAsync(newTeam);
        }
        await _context.SaveChangesAsync();

        await Clients.Group(teamName).SendAsync("ReceiveMessageByGroup", name, team.Id);
    }

    public async Task GetNamesByGroup()
    {
        var teams = await _context.Teams.Include(x => x.Users).Select(x => new
        {
            teamId = x.Id,
            Users = x.Users.Select(u => new
            {
                // Include only necessary user properties
                UserId = u.Id,
                name = u.Name
                // Add other properties as needed
            }).ToList()
        }).ToListAsync();

        await Clients.All.SendAsync("ReceiveNamesByGroup", teams);
    }

    public async Task RemoveFromGroup(string teamName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
    }


    public async override Task OnConnectedAsync()
    {
        ClientCount++;
        await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
        await base.OnConnectedAsync();
    }
    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        ClientCount--;
        await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
        await base.OnDisconnectedAsync(exception);
    }
}
