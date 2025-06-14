using PokemonGenerator.Models;

namespace PokemonGenerator.Services
{
    public interface IUserService
    {
        User GetUser(string email);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
    }
}
