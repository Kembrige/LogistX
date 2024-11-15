using LogistX.Models;


using Microsoft.EntityFrameworkCore;


namespace LogistX.Data
{

    /// <summary>
    /// Контекст базы данных для LogistX.
    /// </summary>
    public class LogistXContext : DbContext
    {
        public LogistXContext(DbContextOptions<LogistXContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Invoice> Invoices { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Определение ограничений и начальных значений
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(c => c.Id);
            });

            modelBuilder.Entity<Models.Route>(entity =>
            {
                entity.HasKey(r => r.Id);

                // Связь маршрутов с водителем (пользователем)
                entity.HasOne(r => r.Driver)
                      .WithMany()
                      .HasForeignKey(r => r.DriverId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(i => i.Id);

                // Связь инвойсов с компаниями и маршрутами
                entity.HasOne(i => i.Company)
                      .WithMany()
                      .HasForeignKey(i => i.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.Route)
                      .WithMany()
                      .HasForeignKey(i => i.RouteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

