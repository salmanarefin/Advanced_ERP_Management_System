using ERP_version1.Models.HR;
using System.ComponentModel.DataAnnotations;

namespace ERP_version1.Models.AttendanceModule
{
    public class Attendance : IValidatableObject
    {
        public int AttendanceId { get; set; }

        [Required(ErrorMessage = "Employee is required.")]
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Attendance date is required.")]
        public DateTime AttendanceDate { get; set; } = DateTime.Today;

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [Range(0, 24, ErrorMessage = "Working hours must be between 0 and 24.")]
        public double WorkingHours { get; set; }

        [Required(ErrorMessage = "Attendance status is required.")]
        public string Status { get; set; } = "Present";

        [Range(0, int.MaxValue, ErrorMessage = "Late minutes cannot be negative.")]
        public int LateMinutes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AttendanceDate.Date > DateTime.Today)
            {
                yield return new ValidationResult(
                    "Attendance date cannot be a future date.",
                    new[] { nameof(AttendanceDate) });
            }

            if (CheckInTime.HasValue && CheckOutTime.HasValue)
            {
                if (CheckOutTime.Value <= CheckInTime.Value)
                {
                    yield return new ValidationResult(
                        "Check-out time must be after check-in time.",
                        new[] { nameof(CheckOutTime) });
                }
            }

            List<string> validStatuses = new List<string>
            {
                "Present",
                "Absent",
                "Late",
                "Half Day"
            };

            if (!validStatuses.Contains(Status))
            {
                yield return new ValidationResult(
                    "Attendance status must be Present, Absent, Late, or Half Day.",
                    new[] { nameof(Status) });
            }

            if (Status == "Absent")
            {
                if (CheckInTime.HasValue || CheckOutTime.HasValue)
                {
                    yield return new ValidationResult(
                        "Absent attendance should not have check-in or check-out time.",
                        new[] { nameof(Status) });
                }

                if (WorkingHours > 0)
                {
                    yield return new ValidationResult(
                        "Absent attendance should not have working hours.",
                        new[] { nameof(WorkingHours) });
                }
            }
        }
    }
}