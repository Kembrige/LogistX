using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using LogistX.Data;
using LogistX.Models;
using BCrypt.Net;
using LogistX.Models.ViewModel;

namespace LogistX.Controllers
{
    public class AccountController : Controller
    {
        private readonly LogistXContext _context;

        public AccountController(LogistXContext context)
        {
            _context = context;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var users = _context.Users.ToList();
                // Найти пользователя по имени
                var user = _context.Users.SingleOrDefault(u => u.UserName == model.UserName);

                // Проверка, что пользователь существует и пароли совпадают
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    // Успешный вход
                    await HttpContext.SignInAsync("CookieAuthentication", new ClaimsPrincipal(
                        new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Role, user.Role.ToString())
                        }, "CookieAuthentication")));

                    return RedirectToAction("Index", "Home");
                }

                // Если проверка не прошла, добавляем ошибку в ModelState
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
            }

            return View(model);
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Создаем нового пользователя
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Role = model.Role
                };

                // Хеширование пароля с помощью BCrypt
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                _context.Users.Add(user);
                _context.SaveChanges();

                // Автоматический вход после регистрации
                await HttpContext.SignInAsync("CookieAuthentication", new ClaimsPrincipal(
                    new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                    }, "CookieAuthentication")));

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuthentication");

            // Отключение кэширования, чтобы исключить проблемы с навбаром
            Response.Headers["Cache-Control"] = "no-cache, no-store";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Index", "Home");
        }
    }
}