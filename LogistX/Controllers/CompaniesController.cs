using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LogistX.Data;
using LogistX.Models;
using Microsoft.AspNetCore.Authorization;

namespace LogistX.Controllers
{
    /// <summary>
    /// Контроллер для управления компаниями.
    /// </summary>
    public class CompaniesController : Controller
    {
        private readonly LogistXContext _context;

        public CompaniesController(LogistXContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies.ToListAsync();
            return View(companies);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Company model)
        {
            if (ModelState.IsValid)
            {
                _context.Companies.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var company = await _context.Companies.FindAsync(id);
            if (company == null) return NotFound();

            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Company model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var company = await _context.Companies.FindAsync(id);
            if (company == null) return NotFound();

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}