using AppointmentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentApi.Data
{
    public class AppointmentsDbContext : DbContext
    {
        public AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure Appointment table
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Title).IsRequired();
                entity.Property(a => a.StartTime).IsRequired();
                entity.Property(a => a.EndTime).IsRequired();
                
                // Explicitly configure Priority field as int
                entity.Property(a => a.Priority)
                    .IsRequired()
                    .HasDefaultValue(0); // 0 = low
            });
        }
    }
}
