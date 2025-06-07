using MongoDB.Driver;
using PokemonGenerator.Models;
using Microsoft.Extensions.Options;
using PokemonGenerator.Config;


namespace PokemonGenerator.Services
{
    public class StoredPokemonService
    {
        private readonly IMongoCollection<OwnedPokemon> _collection;

        public StoredPokemonService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<OwnedPokemon>(settings.Value.CollectionName);
        }

        public async Task SavePokemonListAsync(List<OwnedPokemon> pokemons)
        {
            await _collection.InsertManyAsync(pokemons);
        }

        public async Task<List<OwnedPokemon>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task DeletePokemonAsync(string id)
        {
            var filter = Builders<OwnedPokemon>.Filter.Eq(p => p.Id, id);
            await _collection.DeleteOneAsync(filter);
        }

        /*Delete all stored pokemon functionality*/
        public async Task DeleteAllAsync()
        {
            await _collection.DeleteManyAsync(_ => true);
        }

        /*Functionality for toggling favorite pokemon*/
        public async Task ToggleFavoriteAsync(string id)
        {
            var filter = Builders<OwnedPokemon>.Filter.Eq(p => p.Id, id);
            var pokemon = await _collection.Find(filter).FirstOrDefaultAsync();

            if (pokemon != null)
            {
                var update = Builders<OwnedPokemon>.Update.Set(p => p.IsFavorite, !pokemon.IsFavorite);
                await _collection.UpdateOneAsync(filter, update);
            }
        }

    }
}
