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

        public User GetUser(string email)
        {
            return _users.Find(u => u.Email == email).FirstOrDefault();
        }

        public void UpdateUser(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            _users.ReplaceOne(filter, user);
        }

        public void DeleteUser(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            _users.DeleteOne(filter);
        }
    }
}
