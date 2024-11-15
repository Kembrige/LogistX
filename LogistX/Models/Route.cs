using System;
using System.ComponentModel.DataAnnotations;

namespace LogistX.Models
{
    /// <summary>
    /// Модель маршрута.
    /// </summary>
    public class Route
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите место отправления.")]
        public string StartLocation { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите место назначения.")]
        public string EndLocation { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите дату начала.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите дату завершения.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Пожалуйста, выберите водителя.")]
        public int? DriverId { get; set; }

        public User Driver { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите статус маршрута.")]
        public string RouteStatus { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите цену за тонну.")]
        public int PricePerTon { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите расстояние.")]
        public int Distance { get; set; }
    }
}




