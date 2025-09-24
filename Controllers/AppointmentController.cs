using AppointmentApi.Business;
using AppointmentApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentBL _appointmentBL;

        public AppointmentController(IAppointmentBL appointmentBL)
        {
            _appointmentBL = appointmentBL;
        }

        // GET: api/Appointment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> Get([FromQuery] DateTime? date)
        {
            try
            {
                IEnumerable<Appointment> appointments;
                
                if (date.HasValue)
                {
                    // Calculate the first day of the previous month
                    DateTime startDate = new DateTime(date.Value.Year, date.Value.Month, 1).AddMonths(-1);
                    
                    // Calculate the last day of the next month
                    DateTime endDate = new DateTime(date.Value.Year, date.Value.Month, 1).AddMonths(2).AddDays(-1);
                    
                    // Get appointments for the three-month period
                    appointments = await _appointmentBL.GetAppointmentsByDateRangeAsync(startDate, endDate);
                }
                else
                {
                    // If no date is provided, return all appointments
                    appointments = await _appointmentBL.GetAllAppointmentsAsync();
                }

                // Convert to DTO with string priority
                var appointmentDTOs = appointments.Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Priority = a.PriorityString // Use the string representation
                });

                return Ok(appointmentDTOs);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while retrieving appointments", error = ex.Message });
            }
        }

        // GET: api/Appointment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDTO>> Get(int id)
        {
            var appointment = await _appointmentBL.GetAppointmentByIdAsync(id);
            
            if (appointment == null)
            {
                return NotFound();
            }

            // Convert to DTO with string priority
            var appointmentDTO = new AppointmentDTO
            {
                Id = appointment.Id,
                Title = appointment.Title,
                Description = appointment.Description,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Priority = appointment.PriorityString // Use the string representation
            };

            return Ok(appointmentDTO);
        }
        // PUT: api/Appointment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] AppointmentDTO appointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != appointmentDTO.Id)
            {
                return BadRequest("ID mismatch between route and appointment data");
            }

            // Check if appointment exists
            var existingAppointment = await _appointmentBL.GetAppointmentByIdAsync(id);
            if (existingAppointment == null)
            {
                return NotFound();
            }

            // Create appointment object from DTO
            var appointment = new Appointment
            {
                Id = appointmentDTO.Id,
                Title = appointmentDTO.Title,
                Description = appointmentDTO.Description,
                StartTime = appointmentDTO.StartTime,
                EndTime = appointmentDTO.EndTime
            };

            // Set priority using the string representation
            appointment.PriorityString = appointmentDTO.Priority ?? "low";

            var (success, conflicts) = await _appointmentBL.UpdateAppointmentAsync(appointment);
            
            if (!success)
            {
                if (conflicts.Any())
                {
                    return Conflict(new { message = "Time slot already booked", conflicts });
                }
                return NotFound();
            }

            // Convert back to DTO with string priority for response
            var updatedAppointmentDTO = new AppointmentDTO
            {
                Id = appointment.Id,
                Title = appointment.Title,
                Description = appointment.Description,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Priority = appointment.PriorityString
            };

            return Ok(updatedAppointmentDTO);
        }

        // POST: api/Appointment
        [HttpPost]
        public async Task<ActionResult<AppointmentDTO>> Post([FromBody] AppointmentDTO appointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appointment = new Appointment
            {
                Title = appointmentDTO.Title,
                Description = appointmentDTO.Description,
                StartTime = appointmentDTO.StartTime,
                EndTime = appointmentDTO.EndTime
            };

            // Set priority using the string representation
            appointment.PriorityString = appointmentDTO.Priority ?? "low";

            var (success, conflicts) = await _appointmentBL.AddAppointmentAsync(appointment);
            
            if (!success)
            {
                return Conflict(new { message = "Time slot already booked", conflicts });
            }

            // Convert back to DTO with string priority
            var createdAppointmentDTO = new AppointmentDTO
            {
                Id = appointment.Id,
                Title = appointment.Title,
                Description = appointment.Description,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Priority = appointment.PriorityString // Use the string representation
            };

            return CreatedAtAction(nameof(Get), new { id = appointment.Id }, createdAppointmentDTO);
        }

        // DELETE: api/Appointment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _appointmentBL.DeleteAppointmentAsync(id);
            
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }

    // DTO to handle string representation of priority
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Priority { get; set; } = "low";
    }
}
