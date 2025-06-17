using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PokemonGenerator.Config;
using PokemonGenerator.Models;
using System.Diagnostics;

namespace PokemonGenerator.Services
{
    public class TradeService
    {
        private readonly IMongoCollection<TradeModel> _tradeCollection;

        public TradeService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _tradeCollection = database.GetCollection<TradeModel>("Trades");
        }

        public async Task CreateTradeAsync(TradeModel trade)
        {
            await _tradeCollection.InsertOneAsync(trade);
        }

        public async Task<List<TradeModel>> GetTradesForUserAsync(string userId)
        {
            var filter = Builders<TradeModel>.Filter.Or(
                Builders<TradeModel>.Filter.Eq(t => t.RequesterUserId, userId),
                Builders<TradeModel>.Filter.Eq(t => t.ReceiverUserId, userId)
            );

            return await _tradeCollection.Find(filter).ToListAsync();
        }

        public async Task<TradeModel?> GetByIdAsync(string id)
        {
            return await _tradeCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateTradeStatusAsync(string tradeId, StatusType newStatus)
        {

            var filter = Builders<TradeModel>.Filter.Eq(t => t.Id, tradeId);
            var update = Builders<TradeModel>.Update.Set(t => t.Status, newStatus);
            await _tradeCollection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteTradeAsync(string id)
        {
            await _tradeCollection.DeleteOneAsync(t => t.Id == id);
        }
    }
}
