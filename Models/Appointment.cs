namespace AppointmentApi.Models
{
    public class Appointment
    {
        public int Id { get; set; }            // Unique identifier for the appointment
        public string Title { get; set; }      // Title or purpose of the appointment
        public string Description { get; set; } // Optional description/details
        public DateTime StartTime { get; set; } // When the appointment starts
        public DateTime EndTime { get; set; }   // When the appointment ends
    }
}
