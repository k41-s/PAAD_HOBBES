using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokeApiNet;
using RandomPokemon.Models;

namespace RandomPokemon.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class PokemonController : Controller
    {
        private readonly PokeApiClient _client;
        private readonly string _rootUrl = "https://pokeapi.co/api/v2";
        private Random _random;

        public PokemonController(PokeApiClient client)
        {
            _client = client;
            _random = new Random();
        }

        [HttpGet]
        // GET: PokemonController
        public async Task<ActionResult> Index()
        {
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));

            return View();
        }

        [HttpGet("[Action]")]
        public async Task<ActionResult> GetRandomPokemon()
        {
            Pokemon pokemon = null;
            bool found = false;

            while (!found)
            {
                try
                {
                    int rand = _random.Next(0, 1303); // [0, 1303>
                    pokemon = await _client.GetResourceAsync<Pokemon>(rand);
                    found = true;
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            if (pokemon == null)
                ViewBag.Error = "Could not find valid pokemon, please try again";

            ViewBag.Pokemon = pokemon;

            return View("Index");
        }

        // GET: PokemonController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PokemonController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PokemonController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PokemonController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PokemonController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PokemonController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PokemonController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
