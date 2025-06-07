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

        public PokemonController(PokemonService pokemonService, StoredPokemonService storedPokemonService)
        {
            _pokemonService = pokemonService;
            _storedPokemonService = storedPokemonService;
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
            var pokemons = await _pokemonService.GetRandomPokemonAsync();

            var stored = pokemons.Select(p => new OwnedPokemon
            {
                Name = p.Name,
                SpriteUrl = p.Sprites.FrontDefault
            }).ToList();

            await _storedPokemonService.SavePokemonListAsync(stored);

            return RedirectToAction("Index");
        }


        /*Testing Database*/
        [HttpGet]
        public async Task<IActionResult> ViewStored()
        {
            var pokemons = await _storedPokemonService.GetAllAsync();
            return View("Owned", pokemons);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await _storedPokemonService.DeletePokemonAsync(id);
            return RedirectToAction("Index");
        }


        /*Delete all stored pokemon functionality*/
        [HttpPost]
        public async Task<IActionResult> DeleteAll()
        {
            await _storedPokemonService.DeleteAllAsync();
            return RedirectToAction("ViewStored");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(string id)
        {
            await _storedPokemonService.ToggleFavoriteAsync(id);
            return Ok();
        }

    }
}
