using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PokemonGenerator.Config;
using PokemonGenerator.Models;

namespace PokemonGenerator.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        public UserService(IOptions<MongoDbSettings> settings)
        {
            MongoClient client = new MongoClient(settings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<User>("Users");
        }

        public void CreateUser(User user)
        {
            _users.InsertOne(user);
        }

        public User GetUser(string email)
        {
            return _users.Find(u => u.Username == email).FirstOrDefault();
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

        public User GetUserById(string id)
        {
            return _users.Find(u => u.Id == id).FirstOrDefault();
        }

        public List<User> GetAllUsersExceptAsync(string userId)
        {
            return _users.Find(u => u.Id != userId).ToList();
        }
    }
}
