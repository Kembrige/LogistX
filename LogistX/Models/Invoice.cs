using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogistX.Models
{
    /// <summary>
    /// Модель инвойса.
    /// </summary>
    public class Invoice
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Компания обязательна.")]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [Required(ErrorMessage = "Маршрут обязателен.")]
        public int RouteId { get; set; }
        public Route Route { get; set; }

        [Required(ErrorMessage = "Сумма обязательна.")]
        [Column(TypeName = "decimal(18, 2)")]
        public int Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
        public string PDFPath { get; set; } = string.Empty;
    }
}
