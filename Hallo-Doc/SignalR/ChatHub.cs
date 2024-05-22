using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
namespace Hallo_Doc.SignalR
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, string> getConnection = new Dictionary<string, string>();
        private readonly ApplicationDbContext _context;
        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public override async Task OnConnectedAsync()
        {
            string jwtToken = Context.GetHttpContext().Request.Cookies["jwt"];
            string id = "";
            if (!string.IsNullOrEmpty(jwtToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadToken(jwtToken) as JwtSecurityToken;

                if (token != null)
                {
                    id = token.Claims.FirstOrDefault(claim => claim.Type == "UserID")?.Value ?? "";
                }
            }
            getConnection[Context.ConnectionId] = id;
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            getConnection.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string user, string message)
        {
            var connectionId = getConnection.FirstOrDefault(x => x.Value == user).Key;
            var currentUser = getConnection.FirstOrDefault(u => u.Key == Context.ConnectionId).Value;
            await Clients.Clients(connectionId).SendAsync("ReceiveMessage", user, message);
            if (connectionId != null)
            {
                
                ChatHistory chat = new ChatHistory()
                {
                    Sender = currentUser,
                    Reciever = user,
                    Message = message,
                    IsRead = true,
                    CreatedDate = DateTime.Now,
                };
                _context.ChatHistories.Add(chat);
                _context.SaveChanges();
            }
            else
            {
                ChatHistory chat = new ChatHistory()
                {
                    Sender = currentUser,
                    Reciever = user,
                    Message = message,
                    IsRead = false,
                    CreatedDate = DateTime.Now,
                };
                _context.ChatHistories.Add(chat);
                _context.SaveChanges();
            }
        }
    }
}
