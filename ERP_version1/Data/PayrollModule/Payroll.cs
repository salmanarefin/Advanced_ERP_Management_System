using ERP_version1.Models.HR;
using System.ComponentModel.DataAnnotations;

namespace ERP_version1.Models.PayrollModule
{
    public class Payroll
    {
        public int PayrollId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        public decimal BasicSalary { get; set; }

        public decimal Bonus { get; set; }

        public decimal Deduction { get; set; }

        public decimal Tax { get; set; }

        public decimal NetSalary { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }
}