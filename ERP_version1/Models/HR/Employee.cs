using System.ComponentModel.DataAnnotations;

namespace ERP_version1.Models.HR
{
    public class Employee : IValidatableObject
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee code is required.")]
        [StringLength(20, ErrorMessage = "Employee code cannot be more than 20 characters.")]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot be more than 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "Joining date is required.")]
        public DateTime JoiningDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Basic salary cannot be negative.")]
        public decimal BasicSalary { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        [Required(ErrorMessage = "Designation is required.")]
        public int DesignationId { get; set; }

        public Designation? Designation { get; set; }

        [Required(ErrorMessage = "Employment status is required.")]
        public string EmploymentStatus { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (JoiningDate.Date > DateTime.Today)
            {
                yield return new ValidationResult(
                    "Joining date cannot be a future date.",
                    new[] { nameof(JoiningDate) });
            }

            List<string> validStatuses = new List<string>
            {
                "Active",
                "Inactive",
                "Resigned",
                "Terminated"
            };

            if (!validStatuses.Contains(EmploymentStatus))
            {
                yield return new ValidationResult(
                    "Employment status must be Active, Inactive, Resigned, or Terminated.",
                    new[] { nameof(EmploymentStatus) });
            }
        }
    }
}