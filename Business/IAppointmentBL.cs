using AppointmentApi.Models;
using System.Collections.Generic;

namespace AppointmentApi.Business
{
    public interface IAppointmentBL
    {
        IEnumerable<Appointment> GetAllAppointments();
        Appointment GetAppointmentById(int id);
        void AddAppointment(Appointment appointment);
        void DeleteAppointment(int id);
    }
}
