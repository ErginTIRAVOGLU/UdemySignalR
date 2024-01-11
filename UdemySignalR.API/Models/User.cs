namespace UdemySignalR.API.Models
{
    public class User
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int TeamId {  get; set; }
        public virtual Team? Team { get; set; }
    }
}
