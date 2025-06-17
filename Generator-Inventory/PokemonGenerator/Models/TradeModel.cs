using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PokemonGenerator.Models
{
    public class TradeModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string RequesterUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public string RequesterPokemonId { get; set; }
        public string ReceiverPokemonId { get; set; }
        public StatusType Status { get; set; } = StatusType.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum StatusType
    {
        Pending,
        Accepted,
        Rejected
    }
}
