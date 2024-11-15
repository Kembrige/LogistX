using System.ComponentModel.DataAnnotations;
using System.Text;
using LogistX.Data;
using LogistX.Models;
using LogistX.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class InvoicesController : Controller
{
    private readonly LogistXContext _context;
    private readonly PdfService _pdfService;

    public InvoicesController(LogistXContext context, PdfService pdfService)
    {
        _context = context;
        _pdfService = pdfService;
    }

    public async Task<IActionResult> Index()
    {
        var invoices = await _context.Invoices
            .Include(i => i.Company)
            .Include(i => i.Route)
            .ToListAsync();
        return View(invoices);
    }

    [HttpPost]
    public async Task<IActionResult> GeneratePdf(int invoiceId)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Company)
            .Include(i => i.Route)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);

        if (invoice == null)
        {
            return NotFound();
        }

        var htmlContent = $@"
                <html>
                <body>
                    <h1>Инвойс #{invoice.Id}</h1>
                    <p>Компания: {invoice.Company.Name}</p>
                    <p>Маршрут: {invoice.Route.StartLocation} - {invoice.Route.EndLocation}</p>
                    <p>Цена за тонну: {invoice.Route.PricePerTon}</p>
                    <p>Дистанция: {invoice.Route.Distance}</p>
                    <p>Сумма: {invoice.Amount}</p>
                    <p>Дата: {invoice.Date:dd.MM.yyyy}</p>
                </body>
                </html>";

        var pdfData = _pdfService.GeneratePdf(htmlContent);

        var pdfDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
        if (!Directory.Exists(pdfDirectory))
        {
            Directory.CreateDirectory(pdfDirectory);
        }

        var pdfPath = Path.Combine(pdfDirectory, $"invoice_{invoice.Id}.pdf");
        System.IO.File.WriteAllBytes(pdfPath, pdfData);

        invoice.PDFPath = $"/pdfs/invoice_{invoice.Id}.pdf";
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult DownloadPdf(int invoiceId)
    {
        var invoice = _context.Invoices.FirstOrDefault(i => i.Id == invoiceId);

        if (invoice == null || string.IsNullOrEmpty(invoice.PDFPath))
        {
            return NotFound();
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", invoice.PDFPath.TrimStart('/'));
        var fileBytes = System.IO.File.ReadAllBytes(filePath);

        return File(fileBytes, "application/pdf", $"Invoice_{invoice.Id}.pdf");
    }


    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Companies = new SelectList(_context.Companies.ToList(), "Id", "Name");
        ViewBag.Routes = new SelectList(_context.Routes.ToList(), "Id", "StartLocation");
        return View();
    }


    /// <summary>
    /// POST: Создание нового инвойса.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(Invoice model)
    {
        try
        {
            model.Route = _context.Routes.Where(e => e.Id == model.RouteId).FirstOrDefault() 
                ?? throw new NullReferenceException("Не найден маршрут");
            _context.Invoices.Add(model);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании инвойса: {ex.Message}");
            ModelState.AddModelError(string.Empty, "Не удалось создать инвойс.");
        }

        
        // Если модель не прошла валидацию, возвращаем данные для формы
        ViewBag.Companies = new SelectList(_context.Companies.ToList(), "Id", "Name");
        ViewBag.Routes = new SelectList(_context.Routes.ToList(), "Id", "StartLocation");
        return View(model);
    }

    /// <summary>
    /// Удаление инвойса.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            return NotFound("Инвойс не найден.");
        }

        try
        {
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении инвойса: {ex.Message}");
            ModelState.AddModelError(string.Empty, "Не удалось удалить инвойс.");
            return RedirectToAction(nameof(Index));
        }
    }


    //DownloadPDF
    /// <summary>
    /// Post-запрос получения отчета
    /// </summary>
    /// <param name="jsonStructure">Строка json</param>
    /// <returns>Скачивание из браузера файла</returns>
    [HttpGet]
    public async Task<IActionResult> DownloadPDF(int Id)
    {
        var invoice = _context.Invoices.Where(e => e.Id == Id).FirstOrDefault();
        invoice.Route = _context.Routes.Where(e => e.Id == invoice.RouteId).FirstOrDefault();
        invoice.Company = _context.Companies.Where(e => e.Id == invoice.CompanyId).FirstOrDefault();


        return File(
                 PdfService.toPDF(invoice)
                 , "application/pdf"
                 , $"{User.Identity.Name}_Invoice_{DateTime.Now.ToString("mmHHMMyyyy")}.pdf");


    }
}
