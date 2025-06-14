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

            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var indexKeys = Builders<OwnedPokemon>.IndexKeys.Ascending(p => p.UserId);
            var indexModel = new CreateIndexModel<OwnedPokemon>(indexKeys);
            _pokemonCollection.Indexes.CreateOne(indexModel);
        }

        // UPDATED METHODS TO WORK PER USER

        public async Task SavePokemonListAsync(List<OwnedPokemon> pokemons, string userId)
        {
            pokemons.ForEach(p => {
                p.UserId = userId;
            });

            await _pokemonCollection.InsertManyAsync(pokemons);
        }

        public async Task<List<OwnedPokemon>> GetAllAsync(string userId)
        {
            return await _pokemonCollection.Find(p => p.UserId== userId).ToListAsync();
        }


        public async Task DeletePokemonAsync(string id, string userId)
        {
            var filter = Builders<OwnedPokemon>.Filter.And(
                Builders<OwnedPokemon>.Filter.Eq(p => p.Id, id),
                Builders<OwnedPokemon>.Filter.Eq(p => p.UserId, userId)
            );

            await _pokemonCollection.DeleteOneAsync(filter);
        }

        /*Delete all stored pokemon functionality*/
        public async Task DeleteAllAsync(string userId)
        {
            await _pokemonCollection.DeleteManyAsync(p => p.UserId == userId);
        }

        /*Functionality for toggling favorite pokemon*/
        public async Task ToggleFavoriteAsync(string id, string userId)
        {
            var filter = Builders<OwnedPokemon>.Filter.And(
                Builders<OwnedPokemon>.Filter.Eq(p => p.Id, id),
                Builders<OwnedPokemon>.Filter.Eq(p => p.UserId, userId)
            );

            var pokemon = await _pokemonCollection.Find(filter).FirstOrDefaultAsync();

            if (pokemon != null)
            {
                var update = Builders<OwnedPokemon>.Update
                    .Set(p => p.IsFavorite, !pokemon.IsFavorite);

                await _pokemonCollection.UpdateOneAsync(filter, update);
            }
        }

    }
}
