namespace PokemonGenerator.Models
{
    public class MyTradesViewModel
    {
        public List<TradeInboxViewModel> AcceptedTrades { get; set; } = new();
        public List<TradeInboxViewModel> RejectedTrades { get; set; } = new();
    }
}
