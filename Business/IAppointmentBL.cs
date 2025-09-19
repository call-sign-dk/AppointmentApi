using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentApi.Models;

namespace AppointmentApi.Business
{
     public interface IAppointmentBL
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Appointment? GetAppointmentById(int id);
        Task<(bool Success, List<Appointment> Conflicts)> AddAppointmentAsync(Appointment appointment);
        bool AddAppointment(Appointment appointment);
        Task<bool> DeleteAppointmentAsync(int id);
        bool DeleteAppointment(int id);
    }
}
