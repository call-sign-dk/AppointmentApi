using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentApi.Models;

namespace AppointmentApi.Business
{
    public interface IAppointmentBL
    {
        /// <summary>
        /// Returns all appointments from the database.
        /// </summary>
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();

        /// <summary>
        /// Returns all appointments for a specific date.
        /// </summary>
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);

        /// <summary>
        /// Returns a single appointment by its ID (async).
        /// </summary>
        Task<Appointment?> GetAppointmentByIdAsync(int id);

        /// <summary>
        /// Returns a single appointment by its ID (sync).
        /// </summary>
        Appointment? GetAppointmentById(int id);

        /// <summary>
        /// Attempts to create a new appointment.
        /// Returns (Success, Conflicts) tuple, where:
        /// - Success = false if any conflicts were found.
        /// - Conflicts = list of appointments that overlap with the requested time.
        /// </summary>
        Task<(bool Success, List<Appointment> Conflicts)> AddAppointmentAsync(Appointment appointment);

        /// <summary>
        /// Same as AddAppointmentAsync but synchronous version.
        /// </summary>
        bool AddAppointment(Appointment appointment);

        /// <summary>
        /// Deletes an appointment by ID (async).
        /// Returns true if deletion succeeded, false if not found.
        /// </summary>
        Task<bool> DeleteAppointmentAsync(int id);

        /// <summary>
        /// Deletes an appointment by ID (sync).
        /// Returns true if deletion succeeded, false if not found.
        /// </summary>
        bool DeleteAppointment(int id);
    }
}
