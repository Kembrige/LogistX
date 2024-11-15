using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LogistX.Data;
using LogistX.Models;
using Route = LogistX.Models.Route;

namespace LogistX.Controllers
{
    namespace LogistX.Controllers
    {
        /// <summary>
        /// Контроллер для управления маршрутами.
        /// </summary>
        public class RoutesController : Controller
        {
            private readonly LogistXContext _context;

            public RoutesController(LogistXContext context)
            {
                _context = context;
            }

            // Список маршрутов
            public async Task<IActionResult> Index()
            {
                var routes = await _context.Routes
                    .Include(r => r.Driver)
                    .ToListAsync();
                return View(routes);
            }

            // Создание маршрута (GET)
            [HttpGet]
            public IActionResult Create()
            {
                ViewBag.Drivers = new SelectList(_context.Users.Where(u => u.Role == "Driver"), "Id", "UserName");
                ViewBag.StatusOptions = new SelectList(new[] { "Created", "InProgress", "Complete" });
                return View();
            }

            // Создание маршрута (POST)
            [HttpPost]
            public async Task<IActionResult> Create(Route model)
            {
                try
                {
                    // Проверка водителя
                    model.Driver = await _context.Users.FindAsync(model.DriverId);
                    if (model.Driver == null)
                    {
                        ModelState.AddModelError("DriverId", "Пожалуйста, выберите водителя.");
                        ViewBag.Drivers = new SelectList(_context.Users.Where(u => u.Role == "Driver"), "Id", "UserName");
                        ViewBag.StatusOptions = new SelectList(new[] { "Created", "InProgress", "Complete" });
                        return View(model);
                    }

                    // Установка значений
                    if (string.IsNullOrWhiteSpace(model.StartLocation) || string.IsNullOrWhiteSpace(model.EndLocation))
                    {
                        ModelState.AddModelError(string.Empty, "Начальная и конечная точки маршрута обязательны.");
                        ViewBag.Drivers = new SelectList(_context.Users.Where(u => u.Role == "Driver"), "Id", "UserName");
                        ViewBag.StatusOptions = new SelectList(new[] { "Created", "InProgress", "Complete" });
                        return View(model);
                    }

                    if (model.StartDate >= model.EndDate)
                    {
                        ModelState.AddModelError(string.Empty, "Дата начала маршрута должна быть раньше даты окончания.");
                        ViewBag.Drivers = new SelectList(_context.Users.Where(u => u.Role == "Driver"), "Id", "UserName");
                        ViewBag.StatusOptions = new SelectList(new[] {"Created", "InProgress", "Complete" });
                        return View(model);
                    }

                    if (model.Distance <= 0 || model.PricePerTon <= 0)
                    {
                        ModelState.AddModelError(string.Empty, "Расстояние и цена за тонну должны быть больше нуля.");
                        ViewBag.Drivers = new SelectList(_context.Users.Where(u => u.Role == "Driver"), "Id", "UserName");
                        ViewBag.StatusOptions = new SelectList(new[] {"Created", "InProgress", "Complete" });
                        return View(model);
                    }

                    // Устанавливаем статус по умолчанию, если не задан
                    if (string.IsNullOrEmpty(model.RouteStatus))
                    {
                        model.RouteStatus = "Created";
                    }

                    _context.Routes.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при сохранении маршрута: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Ошибка сохранения маршрута.");
                }

                ViewBag.Drivers = new SelectList(_context.Users.Where(u => u.Role == "Driver"), "Id", "UserName");
                ViewBag.StatusOptions = new SelectList(new[] {"Created", "InProgress", "Complete" });
                return View(model);
            }

            // Детали маршрута
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                    return NotFound();

                var route = await _context.Routes
                    .Include(r => r.Driver)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (route == null)
                    return NotFound();

                return View(route);
            }

            // Редактирование маршрута (GET)
            [HttpGet]
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                    return NotFound();

                var route = await _context.Routes.FindAsync(id);
                if (route == null)
                    return NotFound();

                ViewBag.Drivers = new SelectList(_context.Users.Where(u => u.Role == "Driver"), "Id", "UserName", route.DriverId);
                ViewBag.StatusOptions = new SelectList(new[] {"Created", "InProgress", "Complete" }, route.RouteStatus);
                return View(route);
            }

            // Редактирование маршрута (POST)
            [HttpPost]
            public async Task<IActionResult> Edit(int id, Route model)
            {
                if (id != model.Id)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(model);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_context.Routes.Any(e => e.Id == model.Id))
                            return NotFound();
                        else
                            throw;
                    }
                }

                ViewBag.Drivers = new SelectList(_context.Users.Where(u => u.Role == "Driver"), "Id", "UserName", model.DriverId);
                ViewBag.StatusOptions = new SelectList(new[] {"Created", "InProgress", "Complete" }, model.RouteStatus);
                return View(model);
            }

            // Удаление маршрута (GET)
            [HttpGet]
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                    return NotFound();

                var route = await _context.Routes
                    .Include(r => r.Driver)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (route == null)
                    return NotFound();

                return View(route);
            }

            // Удаление маршрута (POST)
            [HttpPost, ActionName("Delete")]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var route = await _context.Routes.FindAsync(id);
                if (route != null)
                {
                    _context.Routes.Remove(route);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
        }
    }
}