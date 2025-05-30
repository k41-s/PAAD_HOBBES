using MongoDB.Driver;
using RandomPokemon.Models;

namespace RandomPokemon.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        public UserService()
        {
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase database = client.GetDatabase("PokemonDB");
            _users = database.GetCollection<User>("Users");
        }

        public void CreateUser(User user)
        {
            _users.InsertOne(user);
        }

        public User GetUser(string username)
        {
            return _users.Find(u => u.Username == username).FirstOrDefault();
        }
    }
}
