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
