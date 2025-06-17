namespace PokemonGenerator.Models
{
    public class TradeInboxViewModel
    {
        public string TradeId { get; set; }
        public string RequesterEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string RequesterPokemonName { get; set; }
        public string? RequesterPokemonSprite { get; set; }
        public string ReceiverPokemonName { get; set; }
        public string? ReceiverPokemonSprite { get; set; }
        public StatusType Status { get; set; }
    }
}
