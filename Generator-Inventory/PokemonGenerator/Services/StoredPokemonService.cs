using MongoDB.Driver;
using PokemonGenerator.Models;
using Microsoft.Extensions.Options;
using PokemonGenerator.Config;


namespace PokemonGenerator.Services
{
    public class StoredPokemonService
    {
        private readonly IMongoCollection<OwnedPokemon> _pokemonCollection;

        public StoredPokemonService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _pokemonCollection = database.GetCollection<OwnedPokemon>("StoredPokemon");
        }

        public async Task SavePokemonListAsync(List<OwnedPokemon> pokemons)
        {
            await _pokemonCollection.InsertManyAsync(pokemons);
        }

        public async Task<List<OwnedPokemon>> GetAllAsync()
        {
            return await _pokemonCollection.Find(_ => true).ToListAsync();
        }

        public async Task DeletePokemonAsync(string id)
        {
            var filter = Builders<OwnedPokemon>.Filter.Eq(p => p.Id, id);
            await _pokemonCollection.DeleteOneAsync(filter);
        }

        /*Delete all stored pokemon functionality*/
        public async Task DeleteAllAsync()
        {
            await _pokemonCollection.DeleteManyAsync(_ => true);
        }

        /*Functionality for toggling favorite pokemon*/
        public async Task ToggleFavoriteAsync(string id)
        {
            var filter = Builders<OwnedPokemon>.Filter.Eq(p => p.Id, id);
            var pokemon = await _pokemonCollection.Find(filter).FirstOrDefaultAsync();

            if (pokemon != null)
            {
                var update = Builders<OwnedPokemon>.Update.Set(p => p.IsFavorite, !pokemon.IsFavorite);
                await _pokemonCollection.UpdateOneAsync(filter, update);
            }
        }

    }
}
