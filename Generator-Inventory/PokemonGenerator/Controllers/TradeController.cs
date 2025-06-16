using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using PokemonGenerator.Models;
using PokemonGenerator.Services;
using System.Security.Policy;
using System.Text;

namespace PokemonGenerator.Controllers
{
    [Authorize]
    public class TradeController : Controller
    {
        private readonly TradeService _tradeService;
        private readonly StoredPokemonService _storedPokemonService;
        private readonly IUserService _userService;
        private string? CurrentUserId =>
            User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value is string email
            ? _userService.GetUser(email)?.Id
            : null;

        public TradeController(TradeService tradeService, StoredPokemonService storedPokemonService, IUserService userService)
        {
            _tradeService = tradeService;
            _storedPokemonService = storedPokemonService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> CreateTrade(string? receiverUserId)
        {
            if (CurrentUserId == null)
                return Unauthorized();

            // Prepare the CreateTradeViewModel:
            var myPokemon = await _storedPokemonService.GetAllAsync(CurrentUserId);
            var otherUsers = _userService.GetAllUsersExceptAsync(CurrentUserId);
            string? selectedReceiverId = receiverUserId ?? otherUsers.FirstOrDefault()?.Id;

            var receiverPokemon = selectedReceiverId != null
                ? await _storedPokemonService.GetAllAsync(selectedReceiverId)
                : new List<OwnedPokemon>();

            var model = new CreateTradeViewModel
            {
                MyPokemon = myPokemon,
                OtherUsers = otherUsers,
                ReceiverPokemon = receiverPokemon,
                SelectedReceiverUserId = selectedReceiverId,
                ShowModal = !string.IsNullOrEmpty(receiverUserId)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OfferTrade(string offeredPokemonId, string wantedPokemonId, string receiverUserId)
        {
            if (CurrentUserId == null || receiverUserId == CurrentUserId)
                return Unauthorized();

            // Validate pokemon exist and belong to users
            OwnedPokemon? offeredPokemon = await _storedPokemonService.GetByIdAsync(offeredPokemonId, CurrentUserId);
            OwnedPokemon? wantedPokemon = await _storedPokemonService.GetByIdAsync(wantedPokemonId, receiverUserId);

            if (offeredPokemon == null || wantedPokemon == null)
                return BadRequest("Invalid Pokemon IDs");

            TradeModel trade = new TradeModel
            {
                RequesterUserId = CurrentUserId,
                ReceiverUserId = receiverUserId,
                RequesterPokemonId = offeredPokemonId,
                ReceiverPokemonId = wantedPokemonId
            };

            await _tradeService.CreateTradeAsync(trade);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RespondToTrade(string tradeId, bool accept)
        {

            TradeModel? trade = await _tradeService.GetByIdAsync(tradeId);
            if (trade == null || trade.ReceiverUserId != CurrentUserId)
                return Unauthorized();

            if (accept)
            {
                // Swap ownership of pokemon
                bool success = await _storedPokemonService.SwapPokemonAsync(
                    trade.RequesterUserId,
                    trade.ReceiverUserId, 
                    trade.RequesterPokemonId, 
                    trade.ReceiverPokemonId
                );

                if (!success)
                    return BadRequest("Failed to swap pokemon");

                await _tradeService.UpdateTradeStatusAsync(tradeId, StatusType.Accepted);
            }
            else
            {
                await _tradeService.UpdateTradeStatusAsync(tradeId, StatusType.Rejected);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if(CurrentUserId == null)
            {
                return Unauthorized();
            }

            IList<TradeModel> trades = await _tradeService.GetTradesForUserAsync(CurrentUserId);

            var incoming = trades.Where(t => t.ReceiverUserId == CurrentUserId).ToList();
            var outgoing = trades.Where(t => t.RequesterUserId == CurrentUserId).ToList();

            var incomingViewModels = new List<TradeInboxViewModel>();
            var outgoingViewModels = new List<TradeInboxViewModel>();

            foreach (var trade in incoming)
            {
                var requester = _userService.GetUserById(trade.RequesterUserId);
                var receiver = _userService.GetUserById(trade.ReceiverUserId);

                var requesterPokemon = await _storedPokemonService.GetByIdAsync(trade.RequesterPokemonId, trade.RequesterUserId);
                var receiverPokemon = await _storedPokemonService.GetByIdAsync(trade.ReceiverPokemonId, trade.ReceiverUserId);

                incomingViewModels.Add(new TradeInboxViewModel
                {
                    TradeId = trade.Id,
                    RequesterEmail = requester?.Username ?? "(Unknown)",
                    ReceiverEmail = receiver?.Username ?? "(Unknown)",
                    RequesterPokemonName = requesterPokemon?.Name ?? "(Missing)",
                    RequesterPokemonSprite = requesterPokemon?.SpriteUrl,
                    ReceiverPokemonName = receiverPokemon?.Name ?? "(Missing)",
                    ReceiverPokemonSprite = receiverPokemon?.SpriteUrl,
                    Status = trade.Status
                });
            }

            foreach (var trade in outgoing)
            {
                var requester = _userService.GetUserById(trade.RequesterUserId);
                var receiver = _userService.GetUserById(trade.ReceiverUserId);

                var requesterPokemon = await _storedPokemonService.GetByIdAsync(trade.RequesterPokemonId, trade.RequesterUserId);
                var receiverPokemon = await _storedPokemonService.GetByIdAsync(trade.ReceiverPokemonId, trade.ReceiverUserId);

                outgoingViewModels.Add(new TradeInboxViewModel
                {
                    TradeId = trade.Id,
                    RequesterEmail = requester?.Username ?? "(Unknown)",
                    ReceiverEmail = receiver?.Username ?? "(Unknown)",
                    RequesterPokemonName = requesterPokemon?.Name ?? "(Missing)",
                    RequesterPokemonSprite = requesterPokemon?.SpriteUrl,
                    ReceiverPokemonName = receiverPokemon?.Name ?? "(Missing)",
                    ReceiverPokemonSprite = receiverPokemon?.SpriteUrl,
                    Status = trade.Status
                });
            }

            var model = new TradeInboxListViewModel
            {
                IncomingTrades = incomingViewModels,
                OutgoingTrades = outgoingViewModels
            };

            return View(model);
        }

        // Still behaves weird
        [HttpGet]
        public async Task<IActionResult> MyTrades()
        {
            if (CurrentUserId == null)
                return Unauthorized();


            IList<TradeModel> trades = await _tradeService.GetTradesForUserAsync(CurrentUserId);

            // Only want to see sent trades
            IList<TradeModel> sentTrades = trades.Where(t => t.RequesterUserId == CurrentUserId).ToList();

            var acceptedTradesViewModel = new List<TradeInboxViewModel>();
            var rejectedTradesViewModel = new List<TradeInboxViewModel>();

            foreach (var trade in sentTrades)
            {
                if (trade.Status == StatusType.Pending)
                    continue;

                bool tradeAccepted = trade.Status == StatusType.Accepted;

                var requester = _userService.GetUserById(trade.RequesterUserId);
                var receiver = _userService.GetUserById(trade.ReceiverUserId);

                var requesterPokemon = await _storedPokemonService.GetPokemonAsync(trade.RequesterPokemonId);

                var receiverPokemon = await _storedPokemonService.GetPokemonAsync(trade.ReceiverPokemonId);


                if (tradeAccepted)
                {
                    acceptedTradesViewModel.Add(new TradeInboxViewModel
                    {
                        TradeId = trade.Id,
                        RequesterEmail = receiver?.Username ?? "(Unknown)",
                        ReceiverEmail = requester?.Username ?? "(Unknown)",
                        RequesterPokemonName = requesterPokemon?.Name ?? "(Missing)",
                        RequesterPokemonSprite = requesterPokemon?.SpriteUrl,
                        ReceiverPokemonName = receiverPokemon?.Name ?? "(Missing)",
                        ReceiverPokemonSprite = receiverPokemon?.SpriteUrl,
                        Status = trade.Status
                    }); 
                }
                else
                {
                    rejectedTradesViewModel.Add(new TradeInboxViewModel
                    {
                        TradeId = trade.Id,
                        RequesterEmail = requester?.Username ?? "(Unknown)",
                        ReceiverEmail = receiver?.Username ?? "(Unknown)",
                        RequesterPokemonName = requesterPokemon?.Name ?? "(Missing)",
                        RequesterPokemonSprite = requesterPokemon?.SpriteUrl,
                        ReceiverPokemonName = receiverPokemon?.Name ?? "(Missing)",
                        ReceiverPokemonSprite = receiverPokemon?.SpriteUrl,
                        Status = trade.Status
                    });
                }
            }

            var model = new MyTradesViewModel
            {
                AcceptedTrades = acceptedTradesViewModel,
                RejectedTrades = rejectedTradesViewModel
            };

            return View(model);
        }
    }
}
