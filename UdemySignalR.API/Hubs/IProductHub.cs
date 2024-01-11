using UdemySignalR.API.Models;

namespace UdemySignalR.API.Hubs
{
    public interface IProductHub
    {
        Task ReceiveProduct(Product p);
    }
}
