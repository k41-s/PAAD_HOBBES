using PokeApiNet;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PokemonGenerator.Services
{
    public class PokemonService
    {
        private readonly PokeApiClient _pokeClient;
        private readonly Random _random;

        public PokemonService()
        {
            _pokeClient = new PokeApiClient();
            _random = new Random();
        }

        public async Task<List<Pokemon>> GetRandomPokemonAsync()
        {
            List<Pokemon> pokemons = new List<Pokemon>();

            for (int i = 0; i < 6; ++i)
            {
                int randomId = _random.Next(1,1033); /*change back to 1032 if there's issues*/
                var pokemon = await _pokeClient.GetResourceAsync<Pokemon>(randomId);
                pokemons.Add(pokemon);
            }

            return pokemons;
        }
    }
}
