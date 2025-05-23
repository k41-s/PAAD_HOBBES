using Microsoft.AspNetCore.Mvc;
using PokemonGenerator.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using PokeApiNet;

namespace PokemonGenerator.Controllers
{
    public class PokemonController : Controller
    {
        private readonly PokemonService _pokemonService;

        public PokemonController(PokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Pokemon> pokemons = null;

            if (TempData.ContainsKey("PokemonList"))
            {
                var json = TempData["PokemonList"] as string;
                pokemons = System.Text.Json.JsonSerializer.Deserialize<List<Pokemon>>(json);
            }
            
            return View(pokemons);
        }

        [HttpPost]
        public async Task<IActionResult> GetRandomPokemons()
        {
            var pokemons = await _pokemonService.GetRandomPokemonAsync();

            TempData["PokemonList"] = System.Text.Json.JsonSerializer.Serialize(pokemons);

            return RedirectToAction("Index");
        }
    }
}
/*
 [HttpGet]
        public IActionResult Index()
        { 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetRandomPokemons()
        {
            List<Pokemon> pokemons = await _pokemonService.GetRandomPokemonAsync();
            return View("Index", pokemons);
        }*/