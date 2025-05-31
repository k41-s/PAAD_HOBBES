using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RandomPokemon.Models;
using RandomPokemon.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RandomPokemon.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Register()
            => View();

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(AccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            User existingUser = _userService.GetUser(model.Username);
            if(existingUser != null) 
            {
                ViewBag.Error = "Username already exists";
                return View(model);
            }

            User user = new User
            {
                Email = model.Username,
                PasswordHash = PasswordHelper.HashPassword(model.Password)
            };

            _userService.CreateUser(user);

            // Immediately login user
            IList<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirect to home
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null, string? reason = null)
        {
            if (reason == "unauthorized")
                TempData["Message"] = "You must be logged in to access that page";

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountViewModel model, string? returnUrl = null)
        {
            User user = _userService.GetUser(model.Username);
            if (user == null || user.PasswordHash != PasswordHelper.HashPassword(model.Password))
            {
                ViewBag.Error = "Invalid login";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            // Create identity and principal
            IList<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
