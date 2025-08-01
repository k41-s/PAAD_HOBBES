﻿using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;
using System.Text;

namespace PokemonGenerator.Models
{
    public class User
    {
        [BsonId] // maps to mongoDB _id
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("PasswordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("LastPokemonSelectedTime")]
        public DateTime? LastPokemonSelectedTime { get; set; }
    }

    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}
