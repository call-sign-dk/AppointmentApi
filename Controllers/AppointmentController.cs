using AppointmentApi.Business;
using AppointmentApi.Models;
using Microsoft.AspNetCore.Mvc;

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

        // ✅ GET: api/appointment?date=2025-09-16
        /// <summary>
        /// Get all appointments, or filter by date (YYYY-MM-DD).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTime? date)
        {
            if (date.HasValue)
            {
                // Fetch only appointments for this date from DB
                var filtered = await _appointmentBL.GetAppointmentsByDateAsync(date.Value);
                return Ok(filtered);
            }

            var allAppointments = await _appointmentBL.GetAllAppointmentsAsync();
            return Ok(allAppointments);
        }

        // ✅ GET: api/appointment/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var appointment = await _appointmentBL.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return Ok(appointment);
        }

        // ✅ POST: api/appointment
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Appointment appointment)
        {
            // Validate that StartTime < EndTime
            if (appointment.StartTime >= appointment.EndTime)
            {
                return BadRequest(new
                {
                    message = "EndTime must be later than StartTime."
                });
            }

            var (success, conflicts) = await _appointmentBL.AddAppointmentAsync(appointment);

            if (!success)
            {
                return Conflict(new
                {
                    message = "Appointment time conflicts with an existing booking.",
                    conflicts = conflicts.Select(c => new
                    {
                        c.Id,
                        c.Title,
                        c.Description,
                        c.StartTime,
                        c.EndTime
                    })
                });
            }

            return CreatedAtAction(nameof(Get), new { id = appointment.Id }, appointment);
        }

        // ✅ DELETE: api/appointment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _appointmentBL.DeleteAppointmentAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
