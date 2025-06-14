using Microsoft.AspNetCore.Mvc;
using PokemonGenerator.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using PokeApiNet;
using PokemonGenerator.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;

namespace PokemonGenerator.Controllers
{
    [Authorize]
    public class PokemonController : Controller
    {
        private readonly PokemonService _pokemonService;
        private readonly StoredPokemonService _storedPokemonService;
        private readonly IUserService _userService;

        private string? _userId;
        private string? CurrentUserId
        {
            get
            {
                if(_userId == null)
                    _userId = GetCurrentUserID();

                return _userId;
            }
        }

        public PokemonController(PokemonService pokemonService, StoredPokemonService storedPokemonService, IUserService userService)
        {
            _pokemonService = pokemonService;
            _storedPokemonService = storedPokemonService;
            _userService = userService;
        }

        private string? GetCurrentUserID()
        {
            // On login, user email is stored in ClaimTypes.Name in the claim
            string? email = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return null; // No current logged in user saved
            }

            User user = _userService.GetUser(email);

            return user?.Id; // Either userId or null
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Don't pass any Pokémon to the view
        }


        /*
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pokemons = await _storedPokemonService.GetAllAsync();
            return View(pokemons);
        }*/


        [HttpPost]
        public async Task<IActionResult> GetRandomPokemons()
        {
            if (CurrentUserId == null)
                return Unauthorized();

            var pokemons = await _pokemonService.GetRandomPokemonAsync();

            var stored = pokemons.Select(p => new OwnedPokemon
            {
                Name = p.Name,
                SpriteUrl = p.Sprites.FrontDefault
            }).ToList();

            await _storedPokemonService.SavePokemonListAsync(stored, CurrentUserId);

            return RedirectToAction("Index");
        }


        /*Testing Database*/
        [HttpGet]
        public async Task<IActionResult> ViewStored()
        {
            if (CurrentUserId == null)
                return Unauthorized();

            var pokemons = await _storedPokemonService.GetAllAsync(CurrentUserId);
            return View("Owned", pokemons);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (CurrentUserId == null)
                return Unauthorized();

            await _storedPokemonService.DeletePokemonAsync(id, CurrentUserId);
            return RedirectToAction("Index");
        }


        /*Delete all stored pokemon functionality*/
        [HttpPost]
        public async Task<IActionResult> DeleteAll()
        {
            if (CurrentUserId == null)
                return Unauthorized();

            await _storedPokemonService.DeleteAllAsync(CurrentUserId);
            return RedirectToAction("ViewStored");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(string id)
        {
            if (CurrentUserId == null)
                return Unauthorized();

            await _storedPokemonService.ToggleFavoriteAsync(id, CurrentUserId);
            return Ok();
        }

    }
}
