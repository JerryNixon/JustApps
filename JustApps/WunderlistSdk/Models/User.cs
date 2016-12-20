using SQLite;

namespace WunderlistSdk.Models
{
    [Table(nameof(User))]
    internal class User
    {
        public User(Json.User user)
        {
            Id = user.id;
            Name = user.name;
            Email = user.email;
        }

        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
