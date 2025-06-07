using RandomPokemon.Models;

namespace RandomPokemon.Services
{
    public interface IUserService
    {
        User GetUser(string username);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
    }
}
