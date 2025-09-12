using AppointmentApi.Business;
using AppointmentApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AppointmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentBL _appointmentBL;

        public AppointmentController(IAppointmentBL appointmentBL)
        {
            _appointmentBL = appointmentBL;
        }

        // GET: api/appointment
        [HttpGet]
        public IEnumerable<Appointment> Get()
        {
            return _appointmentBL.GetAllAppointments();
        }

        // GET: api/appointment/{id}
        [HttpGet("{id}")]
        public ActionResult<Appointment> Get(int id)
        {
            var appointment = _appointmentBL.GetAppointmentById(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return appointment;
        }

        // POST: api/appointment
        [HttpPost]
        public ActionResult Post([FromBody] Appointment appointment)
        {
            _appointmentBL.AddAppointment(appointment);
            return CreatedAtAction(nameof(Get), new { id = appointment.Id }, appointment);
        }

        // DELETE: api/appointment/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var appointment = _appointmentBL.GetAppointmentById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _appointmentBL.DeleteAppointment(id);
            return NoContent();
        }
    }
}
