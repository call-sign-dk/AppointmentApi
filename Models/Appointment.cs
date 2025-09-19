using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentApi.Models
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-generate ID
        public int Id { get; set; }        // Unique identifier for the appointment
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public DateTime StartTime { get; set; }
        
        [Required]
        public DateTime EndTime { get; set; }
        
        // Changed Priority to int to match database
        public int Priority { get; set; } = 0; // Default to 0 (low)
        
        // Add a non-mapped property to handle string representation
        [NotMapped]
        public string PriorityString 
        { 
            get 
            {
                return Priority switch
                {
                    0 => "low",
                    1 => "medium",
                    2 => "high",
                    _ => "low"
                };
            }
            set
            {
                Priority = value?.ToLower() switch
                {
                    "low" => 0,
                    "medium" => 1,
                    "high" => 2,
                    _ => 0
                };
            }
        }
    }
}
