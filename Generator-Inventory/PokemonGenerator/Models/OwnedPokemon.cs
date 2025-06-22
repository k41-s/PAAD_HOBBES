using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace PokemonGenerator.Models
{
    public class OwnedPokemon
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }
        public int PokemonId { get; set; }
        public string Name { get; set; }
        public string SpriteUrl { get; set; }  /*only storing pokemon name and image url*/
        public bool IsFavorite { get; set; } = false;
        public string UserId { get; set; }
    }
}
