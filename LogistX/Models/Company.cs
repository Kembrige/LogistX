using System.ComponentModel.DataAnnotations;

namespace LogistX.Models
{
    /// <summary>
    /// Модель компании.
    /// </summary>
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}