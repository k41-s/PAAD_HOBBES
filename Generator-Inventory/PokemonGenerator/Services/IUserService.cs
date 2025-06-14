using PokemonGenerator.Models;

namespace PokemonGenerator.Services
{
    public interface IUserService
    {
        User GetUser(string email);
        User GetUserById(string id);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        List<User> GetAllUsersExceptAsync(string userId);
    }
}
