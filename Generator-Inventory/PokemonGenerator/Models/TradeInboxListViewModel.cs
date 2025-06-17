namespace PokemonGenerator.Models
{
    public class TradeInboxListViewModel
    {
        public List<TradeInboxViewModel> IncomingTrades { get; set; } = new();
        public List<TradeInboxViewModel> OutgoingTrades { get; set; } = new();
    }
}
