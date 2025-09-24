using AppointmentApi.Data;
using AppointmentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentApi.Business
{
    public class AppointmentBL : IAppointmentBL
    {
        private readonly AppointmentsDbContext _context;
        
        public AppointmentBL(AppointmentsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetches all appointments ordered by StartTime.
        /// </summary>
        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                             .AsNoTracking()
                             .OrderBy(a => a.StartTime)
                             .ToListAsync();
        }

        /// <summary>
        /// Fetches all appointments that start on a specific date (efficient SQL filtering).
        /// </summary>
        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            return await _context.Appointments
                             .AsNoTracking()
                             .Where(a => a.StartTime.Date == date.Date)
                             .OrderBy(a => a.StartTime)
                             .ToListAsync();
        }

        /// <summary>
        /// Fetches all appointments within a date range (inclusive).
        /// </summary>
        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Appointments
                             .AsNoTracking()
                             .Where(a => a.StartTime.Date >= startDate.Date && a.StartTime.Date <= endDate.Date)
                             .OrderBy(a => a.StartTime)
                             .ToListAsync();
        }

        /// <summary>
        /// Finds appointment by its ID asynchronously.
        /// </summary>
        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments.FindAsync(id);
        }

        /// <summary>
        /// Finds appointment by its ID synchronously.
        /// </summary>
        public Appointment? GetAppointmentById(int id)
        {
            return _context.Appointments.Find(id);
        }

        /// <summary>
        /// Adds an appointment if there are no conflicts.
        /// Returns tuple (Success, Conflicts) for better error reporting.
        /// </summary>
        public async Task<(bool Success, List<Appointment> Conflicts)> AddAppointmentAsync(Appointment appointment)
        {
            // Handle priority conversion if needed
            if (appointment.Priority < 0 || appointment.Priority > 2)
            {
                appointment.Priority = 0; // Default to low if invalid
            }

            var conflicts = await _context.Appointments
                .Where(a => (appointment.StartTime < a.EndTime) &&
                            (appointment.EndTime > a.StartTime))
                .ToListAsync();
                
            if (conflicts.Any())
                return (false, conflicts);
                
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return (true, new List<Appointment>());
        }

        /// <summary>
        /// Synchronous version of AddAppointment (used in non-async scenarios).
        /// </summary>
        public bool AddAppointment(Appointment appointment)
        {
            // Handle priority conversion if needed
            if (appointment.Priority < 0 || appointment.Priority > 2)
            {
                appointment.Priority = 0; // Default to low if invalid
            }

            bool conflict = _context.Appointments.Any(a =>
                (appointment.StartTime < a.EndTime) &&
                (appointment.EndTime > a.StartTime)
            );
            
            if (conflict)
                return false;
                
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Deletes appointment by ID asynchronously.
        /// </summary>
        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;
            
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
        
        // Implement only the async version
        public async Task<(bool Success, List<Appointment> Conflicts)> UpdateAppointmentAsync(Appointment appointment)
        {
              // Check if appointment exists
             var existingAppointment = await _context.Appointments.FindAsync(appointment.Id);
            if (existingAppointment == null)
                return (false, new List<Appointment>());
                
            // Handle priority conversion if needed
            if (appointment.Priority < 0 || appointment.Priority > 2)
            {
                appointment.Priority = 0; // Default to low if invalid
            }

            // Check for conflicts with other appointments (excluding the current one)
            var conflicts = await _context.Appointments
                .Where(a => a.Id != appointment.Id && 
                            (appointment.StartTime < a.EndTime) &&
                            (appointment.EndTime > a.StartTime))
                .ToListAsync();
                    
            if (conflicts.Any())
                return (false, conflicts);
            
            // Update properties
            existingAppointment.Title = appointment.Title;
            existingAppointment.Description = appointment.Description;
            existingAppointment.StartTime = appointment.StartTime;
            existingAppointment.EndTime = appointment.EndTime;
            existingAppointment.Priority = appointment.Priority;
            
            await _context.SaveChangesAsync();
            return (true, new List<Appointment>());
        }

        /// <summary>
        /// Synchronous delete for appointment by ID.
        /// </summary>
        public bool DeleteAppointment(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null) return false;
            
            _context.Appointments.Remove(appointment);
            _context.SaveChanges();
            return true;
        }
    }
}
