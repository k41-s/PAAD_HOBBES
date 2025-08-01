﻿namespace PokemonGenerator.Models
{
    public class CreateTradeViewModel
    {
        public List<OwnedPokemon> MyPokemon { get; set; }
        public List<User> OtherUsers { get; set; }
        public List<OwnedPokemon> ReceiverPokemon { get; set; }
        public string? SelectedReceiverUserId { get; set; }
        public bool ShowModal { get; set; }
    }
}
