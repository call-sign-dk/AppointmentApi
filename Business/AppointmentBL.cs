using AppointmentApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace AppointmentApi.Business
{
    public class AppointmentBL : IAppointmentBL
    {
        // For simplicity, using in-memory list to store appointments
        private static readonly List<Appointment> _appointments = new();

        public IEnumerable<Appointment> GetAllAppointments()
        {
            return _appointments;
        }

        public Appointment GetAppointmentById(int id)
        {
            return _appointments.FirstOrDefault(a => a.Id == id);
        }

        public void AddAppointment(Appointment appointment)
        {
            // Simple Id assignment (incremental)
            appointment.Id = _appointments.Count > 0 ? _appointments.Max(a => a.Id) + 1 : 1;
            _appointments.Add(appointment);
        }

        public void DeleteAppointment(int id)
        {
            var appointment = GetAppointmentById(id);
            if (appointment != null)
            {
                _appointments.Remove(appointment);
            }
        }
    }
}
