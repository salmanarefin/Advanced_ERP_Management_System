using ERP_version1.Data;
using ERP_version1.Models.PayrollModule;
using Microsoft.EntityFrameworkCore;

namespace ERP_version1.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public PayrollService(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<List<Payroll>> GetAllPayrollsAsync()
        {
            return await _context.Payrolls
                .Include(p => p.Employee)
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ThenByDescending(p => p.GeneratedAt)
                .ToListAsync();
        }

        public async Task<Payroll?> GetPayrollDetailsAsync(int id)
        {
            return await _context.Payrolls
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Department)
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Designation)
                .FirstOrDefaultAsync(p => p.PayrollId == id);
        }

        public async Task<string?> GeneratePayrollAsync(Payroll payroll)
        {
            if (payroll.EmployeeId <= 0)
            {
                return "Please select an employee.";
            }

            if (payroll.Month < 1 || payroll.Month > 12)
            {
                return "Month must be between 1 and 12.";
            }

            if (payroll.Year < 2000)
            {
                return "Please enter a valid year.";
            }

            bool alreadyGenerated = await _context.Payrolls.AnyAsync(p =>
                p.EmployeeId == payroll.EmployeeId &&
                p.Month == payroll.Month &&
                p.Year == payroll.Year);

            if (alreadyGenerated)
            {
                return "Payroll already generated for this employee in this month.";
            }

            var employee = await _context.Employees.FindAsync(payroll.EmployeeId);

            if (employee == null)
            {
                return "Employee not found.";
            }

            payroll.BasicSalary = employee.BasicSalary;
            payroll.NetSalary = payroll.BasicSalary + payroll.Bonus - payroll.Deduction - payroll.Tax;
            payroll.GeneratedAt = DateTime.Now;

            if (payroll.NetSalary < 0)
            {
                return "Net salary cannot be negative.";
            }

            _context.Payrolls.Add(payroll);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Create",
                "Payroll",
                "Payrolls",
                payroll.PayrollId,
                $"Payroll generated for {employee.FullName}. Net salary: {payroll.NetSalary:0.00}."
            );

            return null;
        }

        public async Task<Payroll?> GetPayrollForDeleteAsync(int id)
        {
            return await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.PayrollId == id);
        }

        public async Task<bool> DeletePayrollAsync(int id)
        {
            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.PayrollId == id);

            if (payroll == null)
            {
                return false;
            }

            string employeeName = payroll.Employee?.FullName ?? "Unknown Employee";
            decimal netSalary = payroll.NetSalary;

            _context.Payrolls.Remove(payroll);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Delete",
                "Payroll",
                "Payrolls",
                id,
                $"Payroll deleted for {employeeName}. Net salary was {netSalary:0.00}."
            );

            return true;
        }
    }
}